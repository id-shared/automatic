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

bool isPurpleDominated(uint8_t a, uint8_t b, uint8_t g, uint8_t r) {
  return a == 255 && b >= 239 && g <= 127 && r >= 239;
}
bool isKeyHeld(int e) {
  return (GetAsyncKeyState(e) & 0x8000) != 0;
}

void CaptureScreenArea(std::function<bool(uint8_t*, int)> processPixelData, int frame_time, int x, int y, int width, int height) {
  // Initialize D3D11 device and context
  ComPtr<ID3D11Device> device;
  ComPtr<ID3D11DeviceContext> context;
  D3D_FEATURE_LEVEL featureLevel;
  HRESULT hr = D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, nullptr, 0,
    D3D11_SDK_VERSION, &device, &featureLevel, &context);
  if (FAILED(hr)) {
    std::cerr << "Failed to create D3D11 device.\n";
    return;
  }

  // Get DXGI device and adapter
  ComPtr<IDXGIDevice> dxgiDevice;
  device.As(&dxgiDevice);
  ComPtr<IDXGIAdapter> adapter;
  hr = dxgiDevice->GetParent(__uuidof(IDXGIAdapter), &adapter);
  if (FAILED(hr)) {
    std::cerr << "Failed to get IDXGIAdapter.\n";
    return;
  }

  ComPtr<IDXGIOutput> output;
  hr = adapter->EnumOutputs(0, &output);
  if (FAILED(hr)) {
    std::cerr << "Failed to enumerate outputs.\n";
    return;
  }

  ComPtr<IDXGIOutput1> output1;
  hr = output.As(&output1);
  if (FAILED(hr)) {
    std::cerr << "Failed to get IDXGIOutput1.\n";
    return;
  }

  // Duplicate the output (desktop)
  ComPtr<IDXGIOutputDuplication> duplication;
  hr = output1->DuplicateOutput(device.Get(), &duplication);
  if (FAILED(hr)) {
    std::cerr << "Failed to duplicate output.\n";
    return;
  }

  // Prepare the staging texture for reading back data
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
  if (FAILED(hr)) {
    std::cerr << "Failed to create staging texture.\n";
    return;
  }

  while (true) {
    // Capture next frame
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    hr = duplication->AcquireNextFrame(frame_time, &frameInfo, &desktopResource);
    if (hr == DXGI_ERROR_WAIT_TIMEOUT) continue;
    if (FAILED(hr)) {
      std::cerr << "Failed to acquire frame.\n";
      break;
    }

    // Convert the acquired resource to texture
    ComPtr<ID3D11Texture2D> desktopTexture;
    hr = desktopResource.As(&desktopTexture);
    if (FAILED(hr)) {
      std::cerr << "Failed to access desktop texture.\n";
      duplication->ReleaseFrame();
      break;
    }

    // Copy the specific area from the full desktop texture to the staging texture
    D3D11_BOX sourceRegion = { x, y, 0, x + width, y + height, 1 };
    context->CopySubresourceRegion(stagingTexture.Get(), 0, 0, 0, 0, desktopTexture.Get(), 0, &sourceRegion);

    // Map the staging texture to CPU-accessible memory
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    hr = context->Map(stagingTexture.Get(), 0, D3D11_MAP_READ, 0, &mappedResource);
    if (SUCCEEDED(hr)) {
      // Pass the row pitch (stride) along with the data pointer
      processPixelData(static_cast<uint8_t*>(mappedResource.pData), mappedResource.RowPitch);
      context->Unmap(stagingTexture.Get(), 0);
    }
    else {
      std::cerr << "Failed to map staging texture.\n";
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

int main() {
  const int zy = 96;
  const int zx = 96;
  const int sy = (1080 - zy) / 2;
  const int sx = (1920 - zx) / 2;
  const int ay = zy / 2;
  const int ax = zx / 2;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  std::function<bool(uint8_t*, int)> processPixelData = [driver, zy, zx, ax](uint8_t* data, int rowPitch) {
    bool y2_[ax] = {};
    bool y1_[ax] = {};
    bool x2_[ax] = {};
    bool x1_[ax] = {};

    for (int y = 0; y < zy; ++y) {
      for (int x = 0; x < zx; ++x) {
        int offset = y * rowPitch + x * 4;

        uint8_t blue = data[offset];
        uint8_t green = data[offset + 1];
        uint8_t red = data[offset + 2];
        uint8_t alpha = data[offset + 3];
        bool isDominated = isPurpleDominated(alpha, blue, green, red);

        if (isDominated) {
          //std::cout << x << ", " << y << " | " << (int)blue << ", " << (int)green << ", " << (int)red << ", " << (int)alpha << std::endl;
          if (y < (zy / 2)) {
            y1_[ay - y - 1] = true;
          }
          else {
            y2_[y - ay] = true;
          }

          if (x < (zx / 2)) {
            x1_[ax - x - 1] = true;
          }
          else {
            x2_[x - ax] = true;
          }
        }
      }
    }

    int y2 = aIndex(y2_, ay);
    int y1 = zIndex(y1_, ay);
    int x2 = zIndex(x2_, ax);
    int x1 = zIndex(x1_, ax);
    int f3 = +1;
    int f1 = +1;
    int e3 = +1;
    int e1 = +1;

    //std::cout << y1_[0] << " | " << y1_[1] << " | " << y1_[2] << " | " << y1_[3] << std::endl;
    //std::cout << x1 << " | " << x2 << std::endl;

    if (y1 >= e1 || y2 >= e1) {
      if (y1 > e3 || y2 > e3) {
        if (!isKeyHeld(VK_LBUTTON)) {
          y1 > e3 ? Xyloid2::yx(driver, (y1 - e3 - e3) * f3 * -1, 0) : Xyloid2::yx(driver, (y2 - e3 + e3) * f3 * +1, 0);
        }
      }
    }
    if (x1 >= e1 || x2 >= e1) {
      if (x1 > e3 || x2 > e3) {
        if (x1 > e3 && x2 > e3) {
          x2 > x1&& Xyloid2::yx(driver, 0, ((x2 - x1 - e3 + e3) / 2) * f1 * +1);
          x1 > x2&& Xyloid2::yx(driver, 0, ((x1 - x2 - e3 - e3) / 2) * f1 * -1);
        }
        else {
          x2 > e3&& Xyloid2::yx(driver, 0, (x2 - e3 + e3) * f3 * +1);
          x1 > e3&& Xyloid2::yx(driver, 0, (x1 - e3 - e3) * f3 * -1);
        }
      }
    }
    return true;
    };

  CaptureScreenArea(processPixelData, 4, sx, sy, zx, zy);

  return 0;
}
