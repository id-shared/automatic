#include "Contact.hpp"
#include "Device.hpp"
#include "Parallel.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <chrono>
#include <condition_variable>
#include <d3d11.h>
#include <dxgi1_2.h>
#include <functional>
#include <random>
#include <wrl.h>

using Microsoft::WRL::ComPtr;

bool CaptureScreenArea(std::function<bool(uint8_t*, int)> processPixelData, int frame_time, int x, int y, int width, int height) {
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

  while (true) {
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    hr = duplication->AcquireNextFrame(frame_time, &frameInfo, &desktopResource);

    if (hr == DXGI_ERROR_WAIT_TIMEOUT) continue;
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

int speed(int e) {
  int ae = std::abs(e);
  return (ae <= +12 && ae >= +10) ? +3 : (ae <= +9 && ae >= +7) ? +3 - (ae % +2) : (ae <= +6 && ae >= +4) ? +2 : (ae <= +3 && ae >= +1) ? +2 - (ae % +2) : +1;
}

bool main() {
  const int count = std::thread::hardware_concurrency();
  const int wide = +64 * +2;
  const int high = +16 * +2;
  const int each = +2;

  const int __y = (1080 - high) / +2;
  const int __x = (1920 - wide) / +2;

  const int _y = high / +2;
  const int _x = wide / +2;

  bool _rc = false;
  bool _r = false;

  bool _lc = false;
  bool _l = false;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);
  std::function<bool()> lambda = [&_l, &_lc, &_r, &_rc, each]() {
    while (true) {
      _rc = isKeyHeld(VK_RCONTROL);
      _r = isKeyHeld(VK_RBUTTON);
      _lc = isKeyHeld(VK_LCONTROL);
      _l = isKeyHeld(VK_LBUTTON);
      std::this_thread::sleep_for(std::chrono::milliseconds(each));
    }
    return true;
    };

  std::thread t(lambda);

  std::function<bool(uint8_t*, int)> process = [&_l, &_lc, &_r, &_rc, driver](uint8_t* _o, UINT row_pitch) {
    for (int y = +0; y < high; ++y) {
      uint8_t* row_ptr = _o + y * row_pitch;

      for (int x = +0; x < wide; ++x) {
        uint8_t* px = row_ptr + x * +4;

        if (px[+0] >= +251 && px[+1] <= +191 && px[+2] >= +251 && px[+3] == +255) {
          int xy = y - _y +3;
          int xx = x - _x +2;

          Xyloid2::yx(driver, _l ? +0 : xy, xx * speed(xx));

          if (std::abs(xx) <= +1) {
            Xyloid2::e1(driver, true);
            Time::XO(random(+19, +39));
            Xyloid2::e1(driver, false);
          }

          return true;
        }
      }
    }

    return false;
    };

  return CaptureScreenArea(process, each, __x, __y, wide, high);
}
