#include "Contact.hpp"
#include "Device.hpp"
#include "Event.hpp"
#include "Parallel.hpp"
#include "Pattern.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include "Xyloid1.hpp"
#include "Xyloid2.hpp"
#include <algorithm>
#include <chrono>
#include <condition_variable>
#include <d3d11.h>
#include <dxgi1_2.h>
#include <functional>
#include <random>
#include <thread>
#include <wrl.h>

using Microsoft::WRL::ComPtr;

bool CaptureScreenArea(std::function<bool(uint8_t*, UINT)> processPixelData, UINT x, UINT y, UINT width, UINT height, double e) {
  ComPtr<ID3D11Device> device;
  ComPtr<ID3D11DeviceContext> context;
  D3D_FEATURE_LEVEL featureLevel;

  HRESULT hr = D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, D3D11_CREATE_DEVICE_BGRA_SUPPORT, nullptr, 0, D3D11_SDK_VERSION, &device, &featureLevel, &context);
  if (FAILED(hr)) throw hr;

  ComPtr<IDXGIDevice> dxgiDevice;
  device.As(&dxgiDevice);
  ComPtr<IDXGIAdapter> adapter;
  hr = dxgiDevice->GetParent(__uuidof(IDXGIAdapter), &adapter);
  if (FAILED(hr)) throw hr;

  ComPtr<IDXGIOutput> output;
  hr = adapter->EnumOutputs(0, &output);
  if (FAILED(hr)) throw hr;

  ComPtr<IDXGIOutput1> output1;
  hr = output.As(&output1);
  if (FAILED(hr)) throw hr;

  ComPtr<IDXGIOutputDuplication> duplication;
  hr = output1->DuplicateOutput(device.Get(), &duplication);
  if (FAILED(hr)) throw hr;

  D3D11_TEXTURE2D_DESC desc = {};
  desc.Width = width;
  desc.Height = height;
  desc.MipLevels = 1;
  desc.ArraySize = 1;
  desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
  desc.SampleDesc.Count = 1;
  desc.Usage = D3D11_USAGE_STAGING;
  desc.BindFlags = 0;
  desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;

  ComPtr<ID3D11Texture2D> stagingTexture;
  hr = device->CreateTexture2D(&desc, nullptr, &stagingTexture);
  if (FAILED(hr)) throw hr;

  Parallel::ThreadPool pool(std::thread::hardware_concurrency());

  const UINT frame_time = static_cast<UINT>(round(e));

  double wait = frame_time;

  while (true) {
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    hr = duplication->AcquireNextFrame(frame_time, &frameInfo, &desktopResource);

    if (hr == DXGI_ERROR_WAIT_TIMEOUT) {
      wait = wait + frame_time;
      continue;
    }
    if (FAILED(hr)) throw hr;

    ComPtr<ID3D11Texture2D> desktopTexture;
    hr = desktopResource.As(&desktopTexture);
    if (FAILED(hr)) throw hr;

    D3D11_BOX sourceRegion = { x, y, 0, x + width, y + height, 1 };
    context->CopySubresourceRegion(stagingTexture.Get(), 0, 0, 0, 0, desktopTexture.Get(), 0, &sourceRegion);

    D3D11_MAPPED_SUBRESOURCE mappedResource;
    hr = context->Map(stagingTexture.Get(), 0, D3D11_MAP_READ, 0, &mappedResource);
    if (SUCCEEDED(hr)) {
      pool.enqueue_task([&processPixelData, &mappedResource]() mutable {
        processPixelData((uint8_t*)mappedResource.pData, mappedResource.RowPitch);
        });
      context->Unmap(stagingTexture.Get(), 0);
    }

    duplication->ReleaseFrame();

    Time::XO(wait);
    wait = frame_time;
  }

  return true;
}

bool isKeyHeld(int e) {
  return (GetAsyncKeyState(e) & 0x8000) != +0;
}

int random(int e_1, int e) {
  std::random_device rd;
  std::mt19937 gen(rd());
  std::uniform_int_distribution<> dist(e_1, e);
  return dist(gen);
}

bool is_red(uint8_t* x) {
  return x[+0] <= +63 && x[+1] <= +63 && x[+2] >= (+255 - +4) && x[+3] == +255;
}

int to_int(double e) {
  return static_cast<int>(round(e));
}

bool move(HANDLE x, double e_11, double e_4, double e_3, double e_2, double e_1, bool a) {
  const double from_y = e_2 >= -1 + 1 ? min(+e_4, e_2) : max(-e_4, e_2);
  const double from_x = e_1 >= -1 + 1 ? min(+e_3, e_1) : max(-e_3, e_1);
  const int axis_y = to_int(from_y * e_11);
  const int axis_x = to_int(from_x * e_11);
  return Xyloid2::yx(x, a ? -1 + 1 : axis_y, axis_x);
};

bool taps(HANDLE x, double e, bool& a_1, bool& a) {
  if (a_1) {
    return false;
  }
  else {
    a = true;
    Xyloid2::e1(x, true);
    Xyloid2::e1(x, false);
    Time::XO(e);
    a = false;
    return true;
  }
};

int upon(std::function<bool(int)> z, int i) {
  return z(i) ? upon(z, i - 1) : i;
}

int till(std::function<bool(int)> z, int i) {
  return z(i) ? till(z, i + 1) : i;
}

bool pattern(HANDLE x, int e, bool a) {
  int dy = (a ? +1 : -1) * Pattern::dy(e);
  int dx = (a ? -1 : +1) * Pattern::dx(e);

  return dy == 0 && dx == 0 || Xyloid2::yx(x, dy * +5, dx * +1);
}

int main() {
  Parallel::ThreadPool system(std::thread::hardware_concurrency());

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  double ratio = (+1000 / +365) / +1.5;
  double frame = +1000 / +64;
  double delay = +1000 / +4;

  const int xy = GetSystemMetrics(SM_CYSCREEN);
  const int xx = GetSystemMetrics(SM_CXSCREEN);

  const int ey = xy / +256;
  const int ex = xx / +256;

  const int cy = xy / +16;
  const int cx = xx / +4;

  int _ey = ey / +2;
  int _ex = ex / +2;
  int _cy = cy / +2;
  int _cx = cx / +2;

  bool _r = false;
  bool _l = false;
  bool __ = false;

  std::function<void()> queuing = [&_l, &_r, driver]() {
    Parallel::ThreadPool queue2(1);
    Parallel::ThreadPool queue1(1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&_l, &_r, &at, &queue1, &queue2, driver](UINT e, bool a) {
      if (e == VK_OEM_6) {
        _r = a;

        queue1.enqueue_task([&_r, driver]() mutable {
          Xyloid2::e2(driver, _r);
          });

        return false;
      }

      if (e == VK_OEM_4) {
        if (a) {
          _l = a;

          queue1.enqueue_task([&_l, &at, &queue2, driver]() mutable {
            Xyloid2::e1(driver, _l);

            at = till([&_l, &queue2, driver](int e) {
              const bool back = _l && (size >= e);

              if (back) {
                queue2.enqueue_task([e, driver]() mutable {
                  pattern(driver, e, true);
                  });

                Time::XO(time / +1);

                return back;
              }
              else {
                return back;
              }
              }, at) - 1;
            });

          return false;
        }
        else {
          _l = a;

          queue1.enqueue_task([&_l, &at, &queue2, driver]() mutable {
            Xyloid2::e1(driver, _l);

            at = upon([&_l, &queue2, driver](int e) {
              const bool back = !_l && (+1 <= e);

              if (back) {
                queue2.enqueue_task([e, driver]() mutable {
                  pattern(driver, e, false);
                  });

                Time::XO(time / +2);

                return back;
              }
              else {
                return back;
              }
              }, at) + 1;
            });

          return false;
        }
      }

      return true;
      });
    };

  std::thread thread(queuing);

  std::function<bool(int, int)> does = [&__, &_l, &_r, &delay, &ratio, &system, _cx, _cy, _ex, _ey, driver](int e_1, int e) {
    if (!__ && _r && -_ey <= e_1 && +_ey >= e_1 && -_ex <= e && +_ex >= e) {
      system.enqueue_task([&__, &_l, &delay, &ratio, _cx, _cy, e, e_1, driver]() mutable {
        move(driver, ratio, _cy, _cx, e_1, e, _l);
        taps(driver, delay, _l, __);
        });
      return true;
    }
    else {
      system.enqueue_task([&_l, &ratio, _cx, _cy, e, e_1, driver]() mutable {
        move(driver, ratio, _cx, _cy, e_1, e, _l);
        });
      return true;
    }
    };

  std::function<bool(uint8_t*, UINT, int, int)> apple = [_cx, _cy, _ex, _ey, does](uint8_t* o1, UINT e_2, int e_1, int e) {
    const int ny_ = e_1 / +2;
    const int nx_ = e / +2;

    for (int y = -1 + 1; y < e_1; ++y) {
      uint8_t* _y = o1 + ((_cy - ny_) + y) * e_2;

      for (int x = -1 + 1; x < e; ++x) {
        uint8_t* _x = _y + ((_cx - nx_) + x) * +4;

        if (is_red(_x)) {
          int axis_y = +y + _ey - ny_;
          int axis_x = +x + _ex - nx_;
          return does(axis_y, axis_x);
        }
      }
    }

    return false;
    };

  std::function<bool(uint8_t*, UINT)> process = [cx, cy, apple](uint8_t* o1, UINT e) {
    if (apple(o1, e, cy / +1, cx / +16)) {
      return true;
    }
    else if (apple(o1, e, cy / +1, cx / +4)) {
      return true;
    }
    else if (apple(o1, e, cy / +1, cx / +2)) {
      return true;
    }
    else if (apple(o1, e, cy / +1, cx / +1)) {
      return true;
    }
    else {
      return false;
    }
    };

  CaptureScreenArea(process, (xx - cx) / +2, (xy - cy) / +2, cx, cy, frame);

  return +1;
}
