#include <d3d11.h>
#include <dxgi1_2.h>
#include <wrl.h>
#include <iostream>

using Microsoft::WRL::ComPtr;

void CaptureScreenArea(int x, int y, int width, int height) {
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
    hr = duplication->AcquireNextFrame(16, &frameInfo, &desktopResource);
    if (hr == DXGI_ERROR_WAIT_TIMEOUT) continue; // No new frame yet, try again
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
      // Access pixel data here
      auto* data = static_cast<uint8_t*>(mappedResource.pData);
      uint8_t blue = data[0];
      uint8_t green = data[1];
      uint8_t red = data[2];
      uint8_t alpha = data[3];
      std::cout << "First pixel - B: " << (int)blue << ", G: " << (int)green
        << ", R: " << (int)red << ", A: " << (int)alpha << "\n";
      context->Unmap(stagingTexture.Get(), 0);
    }
    else {
      std::cerr << "Failed to map staging texture.\n";
    }

    duplication->ReleaseFrame();
  }
}

int main() {
  int x = 1, y = 1, width = 10, height = 10;
  CaptureScreenArea(x, y, width, height);
  while(true) {}
  return 0;
}
