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
  return a == 255 && b >= 191 && g >= 95 && g <= 159 && r >= 191;
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
  const int height = 64, width = 64;
  const int y = (1080 - height) / 2;
  const int x = (1920 - width) / 2;
  const int n = width / 2;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  std::function<bool(uint8_t*, int)> processPixelData = [driver, height, width, n](uint8_t* data, int rowPitch) {
    bool y2_[n] = {};
    bool y1_[n] = {};
    bool x2_[n] = {};
    bool x1_[n] = {};

    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        int offset = y * rowPitch + x * 4;

        uint8_t blue = data[offset];
        uint8_t green = data[offset + 1];
        uint8_t red = data[offset + 2];
        uint8_t alpha = data[offset + 3];
        bool isDominated = isPurpleDominated(alpha, blue, green, red);

        if (isDominated) {
          //std::cout << x << ", " << y << " | " << (int)blue << ", " << (int)green << ", " << (int)red << ", " << (int)alpha << std::endl;
          if (y < (height / 2)) {
            y1_[n - y - 1] = true;
          }
          else {
            y2_[y - n] = true;
          }

          if (x < (width / 2)) {
            x1_[n - x - 1] = true;
          }
          else {
            x2_[x - n] = true;
          }
        }
      }
    }

    int y2 = aIndex(y2_, n);
    int y1 = zIndex(y1_, n);
    int ye = +3;
    int yc = +2;
    int ya = +1;

    int x2 = zIndex(x2_, n);
    int x1 = zIndex(x1_, n);
    int xs = +4;
    int xf = +4;
    int xe = +3;
    int xc = +2;
    int xa = +1;

    //std::cout << y1_[0] << " | " << y1_[1] << " | " << y1_[2] << " | " << y1_[3] << std::endl;
    //std::cout << x1 << " | " << x2 << std::endl;

    /*if (y1 > ya || y2 > ya) {
      y2 > yc&& Xyloid2::yx(driver, (y2 - yc) * ye * +1, 0);
      y1 > yc&& Xyloid2::yx(driver, (y1 - yc) * ye * -1, 0);
    }*/

    if (x1 >= xa || x2 >= ya) {
      if (x1 >= xc && x2 >= xc) {
        /*if (x2 > x1) {
          Xyloid2::yx(driver, ((y2 - y1) / 2) * yc * +1, 0);
        }
        else {
          Xyloid2::yx(driver, ((y1 - y2) / 2) * yc * -1, 0);
        }*/
      }
      else {
        x2 > xc && Xyloid2::yx(driver, 0, (x2 - xc) * xe * +1);
        x1 > xc && Xyloid2::yx(driver, 0, (x1 - xc) * xe * -1);
      }
    }

    /*if (x1 >= 1 && x2 >= 1) {
      if (x1 > x2) {
        Xyloid2::yx(driver, 0, ((x1 - x2) / 2) * xf * -1);
      }
      else {
        Xyloid2::yx(driver, 0, ((x2 - x1) / 2) * xf * +1);
      }
    }
    else {
      if (x1 >= 1 || x2 >= 1) {
        if (x1 >= 1) {
          Xyloid2::yx(driver, 0, ((x1 * xf) + xs) * -1);
        }
        else {
          Xyloid2::yx(driver, 0, ((x2 * xf) + xs) * +1);
        }
      }
    }*/

    return true;
    };

  CaptureScreenArea(processPixelData, 4, x, y, width, height);

  return 0;
}
