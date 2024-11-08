#include "Contact.hpp"
#include "Device.hpp"
#include "Event.hpp"
#include "Parallel.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include "Xyloid1.hpp"
#include "Xyloid2.hpp"
#include <chrono>
#include <condition_variable>
#include <d3d11.h>
#include <dxgi1_2.h>
#include <functional>
#include <random>
#include <thread>
#include <wrl.h>

using Microsoft::WRL::ComPtr;

bool CaptureScreenArea(std::function<bool(uint8_t*, UINT)> processPixelData, UINT frame_time, UINT x, UINT y, UINT width, UINT height) {
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

  auto nextFrameTime = std::chrono::steady_clock::now();

  while (true) {
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    hr = duplication->AcquireNextFrame(frame_time, &frameInfo, &desktopResource);

    if (hr == DXGI_ERROR_WAIT_TIMEOUT) {
      nextFrameTime += std::chrono::milliseconds(frame_time);
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

    nextFrameTime += std::chrono::milliseconds(frame_time);
    std::this_thread::sleep_until(nextFrameTime);
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

bool isPurple(uint8_t* x) {
  return x[+0] >= +251 && x[+1] <= +191 && x[+2] >= +251 && x[+3] == +255;
}

int maximum(int e_1, int e) {
  return e >= +1 ? (e_1 <= e ? e_1 : e) : (e_1 >= e ? e_1 : e);
}

bool move(HANDLE x, int e_3, int e_2, int e_1, int e, bool a) {
  return Xyloid2::yx(x, a ? +0 : maximum(e_2 >= +1 ? +e_3 : -e_3, e_2) * e, maximum(e_1 >= +1 ? +e_3 : -e_3, e_1) * e);
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

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  const int system_size = std::thread::hardware_concurrency();

  const int screen_high = GetSystemMetrics(SM_CYSCREEN);
  const int screen_wide = GetSystemMetrics(SM_CXSCREEN);

  const int high = screen_high / +32;
  const int wide = screen_wide / +8;

  const int every = +249;
  const int frame = +1;

  const int ex = (screen_wide - wide) / +2;
  const int ey = (screen_high - high) / +2;

  const int cx = wide / +2;
  const int cy = high / +2;

  UINT az = +64;
  UINT ay = +16;
  UINT ax = +1;

  bool ar = false;
  bool al = false;
  bool a_ = false;

  Parallel::ThreadPool pool_system(system_size);

  Parallel::ThreadPool pool_1(1);

  std::function<void()> queue = [&al, &ar, &ax, &ay, &az, &pool_1, driver]() {
    Event::KeyboardHook hook([&al, &ar, &ax, &ay, &az, &pool_1, driver](UINT e, bool a) {
      if (e == VK_OEM_6) {
        ar = a;

        pool_1.enqueue_task([&ar, driver]() mutable {
          Xyloid2::e2(driver, ar);
          });

        return false;
      }

      if (e == VK_OEM_4) {
        if (a) {
          al = a;

          pool_1.enqueue_task([&al, &ax, &ay, &az, driver]() mutable {
            Xyloid2::e1(driver, al);

            ax = till([&al, &ax, &ay, &az, driver](int ci) {
              return al && (az >= ci) && Time::XO(ay / +1);
              }, ax) + 1;
            });

          return false;
        }
        else {
          al = a;

          pool_1.enqueue_task([&al, &ax, &ay, &az, driver]() mutable {
            Xyloid2::e1(driver, al);

            ax = upon([&al, &ax, &ay, &az, driver](int ci) {
              return !al && (+1 <= ci) && Time::XO(ay / +2);
              }, ax) + 1;
            });

          return false;
        }
      }

      return true;
      });
    };

  std::thread thread(queue);

  std::function<bool(uint8_t*, UINT)> process = [&a_, &al, &ar, &pool_system, cx, cy, high, driver](uint8_t* _o, UINT row_pitch) {
    for (int y = +0; y < (cy * +1.5); ++y) {
      uint8_t* pyu = _o + y * row_pitch;

      for (int x = +0; x < cx; ++x) {
        uint8_t* pxl = pyu + (cx - 1 - x) * +4;
        uint8_t* pxr = pyu + (cx + x) * +4;

        if (isPurple(pxr)) {
          const int move_y = +y - cy + 5;
          const int move_x = +x;

          if (!a_ && ar && move_x <= +4) {
            pool_system.enqueue_task([&a_, &al, high, move_x, move_y, driver]() mutable {
              move(driver, high, move_y, move_x, +2, al);
              taps(driver, every, al, a_);
              });
            return true;
          }
          else {
            pool_system.enqueue_task([&a_, &al, high, move_x, move_y, driver]() mutable {
              move(driver, high, move_y, move_x, +1, al);
              });
            return true;
          }
        }

        if (isPurple(pxl)) {
          const int move_y = +y - cy + 5;
          const int move_x = -x;

          if (!a_ && ar && move_x >= -4) {
            pool_system.enqueue_task([&a_, &al, high, move_x, move_y, driver]() mutable {
              move(driver, high, move_y, move_x, +2, al);
              taps(driver, every, al, a_);
              });
            return true;
          }
          else {
            pool_system.enqueue_task([&a_, &al, high, move_x, move_y, driver]() mutable {
              move(driver, high, move_y, move_x, +1, al);
              });
            return true;
          }
        }
      }
    }
    return false;
    };

  CaptureScreenArea(process, frame, ex, ey, wide, high);

  return +1;
}
