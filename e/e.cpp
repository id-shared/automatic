#include "Contact.hpp"
#include "Device.hpp"
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

bool taps(HANDLE x, double e, bool &a_1, bool& a) {
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

int main() {
  const int system_count = std::thread::hardware_concurrency();

  const int screen_high = GetSystemMetrics(SM_CYSCREEN);
  const int screen_wide = GetSystemMetrics(SM_CXSCREEN);

  const int high = screen_high / +32;
  const int wide = screen_wide / +4;

  const int __x = (screen_wide - wide) / +2;
  const int __y = (screen_high - high) / +2;

  const int _x = wide / +2;
  const int _y = high / +2;

  const int frame = +2;
  const int each = +199;

  bool nx = false;
  bool nr = false;
  bool nl = false;
  bool n_ = false;

  Parallel::ThreadPool pool(std::thread::hardware_concurrency());

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);
  std::function<bool()> lambda = [&nl, &nr]() {
    while (true) {
      nr = isKeyHeld(VK_RBUTTON);
      nl = isKeyHeld(VK_LBUTTON);
    }
    return true;
    };

  std::thread t(lambda);

  std::function<bool(uint8_t*, UINT)> process = [&n_, &nl, &nr, &nx, &pool, _x, _y, high, driver](uint8_t* _o, UINT row_pitch) {
    for (int y = +0; y < high; ++y) {
      uint8_t* row_ptr = _o + y * row_pitch;

      for (int x = +0; x < _x; ++x) {
        uint8_t* pxl = row_ptr + (_x - 1 - x) * +4;
        uint8_t* pxr = row_ptr + (x + _x) * +4;

        if (isPurple(pxr)) {
          const int xy = +y - _y + 4;
          const int xx = +x;

          if (!n_ && nr && (xx > -1 && xx < +1) && (xy > -1 && xy < +4)) {
            move(driver, high, xy, xx, +3, nl);
            pool.enqueue_task([&n_, &nl, driver]() mutable {
              taps(driver, each, nl, n_);
              });
          }
          else {
            move(driver, high, xy, xx, +3, nl);
            pool.enqueue_task([&n_, &nl, driver]() mutable {
              taps(driver, each, nl, n_);
              });
          }

          return true;
        }

        if (isPurple(pxl)) {
          const int xy = +y - _y + 4;
          const int xx = -x;

          if (!n_ && nr && (xx > -1 && xx < +1) && (xy > -1 && xy < +4)) {
            move(driver, high, xy, xx, +3, nl);
            pool.enqueue_task([&n_, &nl, driver]() mutable {
              taps(driver, each, nl, n_);
              });
          }
          else {
            move(driver, high, xy, xx, +3, nl);
            pool.enqueue_task([&n_, &nl, driver]() mutable {
              taps(driver, each, nl, n_);
              });
          }

          return true;
        }
      }
    }

    return false;
    };

  CaptureScreenArea(process, frame, __x, __y, wide, high);

  return +1;
}
