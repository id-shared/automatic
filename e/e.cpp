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

  /*auto nextFrameTime = std::chrono::steady_clock::now();
  nextFrameTime += std::chrono::milliseconds(frame_time);
  nextFrameTime += std::chrono::milliseconds(frame_time);
  std::this_thread::sleep_until(nextFrameTime);*/

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
  return x[+0] <= +63 && x[+1] <= +63 && x[+2] >= (+255 - +1) && x[+3] == +255;
}

bool move(HANDLE x, double e_11, double e_4, double e_3, double e_2, double e_1, bool a) {
  const double from_y = e_2 >= -1 + 1 ? min(+e_4, e_2) : max(-e_4, e_2);
  const double from_x = e_1 >= -1 + 1 ? min(+e_3, e_1) : max(-e_3, e_1);
  const int axis_y = static_cast<int>(round(from_y * e_11));
  const int axis_x = static_cast<int>(round(from_x * e_11));
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

  int screen_y = GetSystemMetrics(SM_CYSCREEN);
  int screen_x = GetSystemMetrics(SM_CXSCREEN);
  double ratio = (+1000 / +365) / +1.0;
  double frame = +1000 / +64;
  double delay = +1000 / +4;

  //const int xy = screen_y / +256;
  //const int xx = screen_x / +256;

  const int xy = +4;
  const int xx = +8;

  const int ey = screen_y / +64;
  const int ex = screen_x / +16;

  const int cy = screen_y / +32;
  const int cx = screen_x / +8;

  bool ar = false;
  bool al = false;
  bool a_ = false;

  std::cout << ratio << ", " << frame << ", " << xx << ", " << xy << std::endl;

  std::function<void()> queuing = [&al, &ar, driver]() {
    Parallel::ThreadPool queue2(1);
    Parallel::ThreadPool queue1(1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&al, &ar, &at, &queue1, &queue2, driver](UINT e, bool a) {
      if (e == VK_OEM_6) {
        ar = a;

        queue1.enqueue_task([&ar, driver]() mutable {
          Xyloid2::e2(driver, ar);
          });

        return false;
      }

      if (e == VK_OEM_4) {
        if (a) {
          al = a;

          queue1.enqueue_task([&al, &at, &queue2, driver]() mutable {
            Xyloid2::e1(driver, al);

            at = till([&al, &queue2, driver](int e) {
              const bool back = al && (size >= e);

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
          al = a;

          queue1.enqueue_task([&al, &at, &queue2, driver]() mutable {
            Xyloid2::e1(driver, al);

            at = upon([&al, &queue2, driver](int e) {
              const bool back = !al && (+1 <= e);

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

  std::function<bool(int, int)> does = [&a_, &al, &ar, &delay, &ratio, &system, xx, xy, driver](int e_1, int e) {
    if (!a_ && ar && e >= -xx && e <= +xx) {
      system.enqueue_task([&a_, &al, &delay, &ratio, xx, xy, e, e_1, driver]() mutable {
        move(driver, ratio, xy, xx, e_1, e, al);
        taps(driver, delay, al, a_);
        });
      return true;
    }
    else {
      system.enqueue_task([&al, &ratio, xx, xy, e, e_1, driver]() mutable {
        move(driver, ratio, xx, xy, e_1, e, al);
        });
      return true;
    }
    };

  std::function<bool(uint8_t*, UINT)> process = [cx, cy, xx, xy, does](uint8_t* o1, UINT e) {
    const int xy_ = xy / +2;
    const int xx_ = xx / +2;

    const int cy_ = cy / +2;
    const int cx_ = cx / +2;

    const int ay = (cy - xy) / +2;
    const int ax = (cx - xx) / +2;

    for (int y_ = -1 + 1; y_ < xy; ++y_) {
      uint8_t* pixel_y = o1 + (ax + y_) * e;

      for (int x_ = -1 + 1; x_ < xx_; ++x_) {
        uint8_t* pixel_l = pixel_y + (cx_ - 1 - x_) * +4;
        uint8_t* pixel_r = pixel_y + (cx_ + x_) * +4;

        if (is_red(pixel_r)) {
          return does(+y_ - xy_, +x_);
        }

        if (is_red(pixel_l)) {
          return does(+y_ - xy_, -x_);
        }
      }
    }

    for (int y = -1 + 1; y < cy; ++y) {
      uint8_t* pixel_y = o1 + y * e;

      for (int x = -1 + 1; x < cx_; ++x) {
        uint8_t* pixel_l = pixel_y + (cx_ - 1 - x) * +4;
        uint8_t* pixel_r = pixel_y + (cx_ + x) * +4;

        if (is_red(pixel_r)) {
          return does(+y - cy_, +x);
        }

        if (is_red(pixel_l)) {
          return does(+y - cy_, -x);
        }
      }
    }

    return false;
    };

  CaptureScreenArea(process, (screen_x - cx) / +2, (screen_y - cy) / +2, cx, cy, frame);

  return +1;
}
