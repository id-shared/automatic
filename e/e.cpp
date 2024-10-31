#include "Contact.hpp"
#include "Device.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <d3d11.h>
#include <dxgi1_2.h>
#include <functional>
#include <iostream>
#include <wrl.h>

using Microsoft::WRL::ComPtr;

void CaptureScreenArea(std::function<bool(uint8_t*, int)> processPixelData, int frame_time, int x, int y, int width, int height) {
  ComPtr<ID3D11Device> device;
  ComPtr<ID3D11DeviceContext> context;
  D3D_FEATURE_LEVEL featureLevel;
  UINT flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_PREVENT_INTERNAL_THREADING_OPTIMIZATIONS;

  HRESULT hr = D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, flags, nullptr, 0, D3D11_SDK_VERSION, &device, &featureLevel, &context);
  FAILED(hr) ? throw hr : hr;

  ComPtr<IDXGIDevice> dxgiDevice;
  device.As(&dxgiDevice);
  ComPtr<IDXGIAdapter> adapter;
  hr = dxgiDevice->GetParent(__uuidof(IDXGIAdapter), &adapter);
  FAILED(hr) ? throw hr : hr;

  ComPtr<IDXGIOutput> output;
  hr = adapter->EnumOutputs(0, &output);
  FAILED(hr) ? throw hr : hr;

  ComPtr<IDXGIOutput1> output1;
  hr = output.As(&output1);
  FAILED(hr) ? throw hr : hr;

  ComPtr<IDXGIOutputDuplication> duplication;
  hr = output1->DuplicateOutput(device.Get(), &duplication);
  FAILED(hr) ? throw hr : hr;

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
  FAILED(hr) ? throw hr : hr;

  while (true) {
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    hr = duplication->AcquireNextFrame(frame_time, &frameInfo, &desktopResource);
    if (hr == DXGI_ERROR_WAIT_TIMEOUT) continue;
    FAILED(hr) ? throw hr : hr;

    ComPtr<ID3D11Texture2D> desktopTexture;
    hr = desktopResource.As(&desktopTexture);
    FAILED(hr) ? throw hr : hr;

    D3D11_BOX sourceRegion = { x, y, 0, x + width, y + height, 1 };
    context->CopySubresourceRegion(stagingTexture.Get(), 0, 0, 0, 0, desktopTexture.Get(), 0, &sourceRegion);

    D3D11_MAPPED_SUBRESOURCE mappedResource;
    hr = context->Map(stagingTexture.Get(), 0, D3D11_MAP_READ, 0, &mappedResource);
    if (SUCCEEDED(hr)) {
      processPixelData(static_cast<uint8_t*>(mappedResource.pData), mappedResource.RowPitch);
      context->Unmap(stagingTexture.Get(), 0);
    }
    else {
      FAILED(hr) ? throw hr : hr;
    }

    duplication->ReleaseFrame();
  }
}

int zIndex(const bool* arr, int size) {
  for (int i = size - 1; i >= 0; --i) {
    if (arr[i]) {
      return i + 1;
    }
  }
  return 0;
}

int aIndex(const bool* arr, int size) {
  for (int i = 0; i < size; ++i) {
    if (arr[i]) {
      return i + 1;
    }
  }
  return 0;
}

bool isKeyHeld(int e) {
  return (GetAsyncKeyState(e) & 0x8000) != 0;
}

int main() {
  const int zz = +2;
  const int zy = 64;
  const int zx = 64;
  const int sy = (1080 - zy) / 2;
  const int sx = (1920 - zx) / 2;
  const int oy = zy / 2;
  const int ox = zx / 2;
  const int f3 = zz;
  const int f1 = zz;
  const int e3 = +1;
  const int e1 = +1;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  std::function<bool(uint8_t*, int)> processPixelData = [driver](uint8_t* _o, int row_pitch) {
    bool y2_[ox] = {}, y1_[ox] = {}, x2_[ox] = {}, x1_[ox] = {};

#pragma omp parallel for
    for (int y = 0; y < zy; ++y) {
      uint8_t* row_ptr = _o + y * row_pitch;

      for (int x = 0; x < zx; ++x) {
        uint8_t* pixel = row_ptr + x * 4;
        //std::cout << static_cast<int>(pixel[0]) << " | " << static_cast<int>(pixel[0]) << " | " << static_cast<int>(pixel[0]) << std::endl;
        if (pixel[0] >= 251 && pixel[1] <= 191 && pixel[2] >= 251 && pixel[3] == 255) {
          int y_idx = y < oy ? oy - y - 1 : y - oy;
          int x_idx = x < ox ? ox - x - 1 : x - ox;

          if (y < oy) y1_[y_idx] = true;
          else        y2_[y_idx] = true;

          if (x < ox) x1_[x_idx] = true;
          else        x2_[x_idx] = true;
        }
      }
    }

    int y2 = aIndex(y2_, oy);
    int y1 = zIndex(y1_, oy);
    int x2 = zIndex(x2_, ox);
    int x1 = zIndex(x1_, ox);

    if (y1 >= e1 || y2 >= e1) {
      if (y1 > e3 || y2 > e3) {
        if (!isKeyHeld(VK_LBUTTON)) {
          y1 > e3 ? Xyloid2::yx(driver, (y1 - e3 - e3) * f3 * -1, 0)
            : Xyloid2::yx(driver, (y2 - e3 + e3) * f3 * +1, 0);
        }
      }
    }
    if (x1 >= e1 || x2 >= e1) {
      if (x1 > e3 || x2 > e3) {
        if (x1 > e3 && x2 > e3) {
          x2 > x1 ? Xyloid2::yx(driver, 0, ((x2 - x1 - e3 + e3) / 2) * f1 * +1)
            : Xyloid2::yx(driver, 0, ((x1 - x2 - e3 - e3) / 2) * f1 * -1);
        }
        else {
          x2 > e3 ? Xyloid2::yx(driver, 0, (x2 - e3 + e3) * f3 * +1)
            : Xyloid2::yx(driver, 0, (x1 - e3 - e3) * f3 * -1);
        }
      }
    }
    return true;
    };

  CaptureScreenArea(processPixelData, zz, sx, sy, zx, zy);

  return 0;
}
