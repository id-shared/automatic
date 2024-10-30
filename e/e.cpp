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

bool isPurpleDominated(uint8_t r, uint8_t g, uint8_t b, double threshold) {
  return (r > threshold * g) && (b > threshold * g);
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

int findLastTrueIndex(const bool* arr, int size) {
  for (int i = size - 1; i >= 0; --i) {
    if (arr[i]) {
      return i + 1;
    }
  }
  return 0;
}

int main() {
  const int height = 2, width = 64;
  const int y = (1080 - height) / 2;
  const int x = (1920 - width) / 2;
  const int n = width / 2;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  std::function<bool(uint8_t*, int)> processPixelData = [driver, height, width, n](uint8_t* data, int rowPitch) {
    bool u[n] = {};
    bool r[n] = {};
    bool l[n] = {};
    bool d[n] = {};

    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        int offset = y * rowPitch + x * 4;

        uint8_t blue = data[offset];
        uint8_t green = data[offset + 1];
        uint8_t red = data[offset + 2];
        uint8_t alpha = data[offset + 3];
        bool isDominated = isPurpleDominated(red, green, blue, 1.75);

        if (isDominated) {
          //std::cout << x << ", " << y << " | " << (int)blue << ", " << (int)green << ", " << (int)red << ", " << (int)alpha << std::endl;
          if (x < (width / 2)) {
            l[n - x - 1] = true;
          }
          else {
            r[x - n] = true;
          }
        }
      }
    }

    int xr = findLastTrueIndex(r, n);
    int xl = findLastTrueIndex(l, n);
    int xf = +5;

    //std::cout << xr << " | " << xl << std::endl;

    if (xl >= 1 && xr >= 1) {
      if (xl > xr) {
        Xyloid2::yx(driver, 0, ((xl - xr) / 2) * xf * -1);
      }
      else {
        Xyloid2::yx(driver, 0, ((xr - xl) / 2) * xf * +1);
      }
    }
    else {
      if (xl >= 1 || xr >= 1) {
        if (xl >= 1) {
          Xyloid2::yx(driver, 0, (xl / 2) * xf * -1);
        }
        else {
          Xyloid2::yx(driver, 0, (xr / 2) * xf * +1);
        }
      }
    }

    return true;
    };

  CaptureScreenArea(processPixelData, 1, x, y, width, height);

  return 0;
}
