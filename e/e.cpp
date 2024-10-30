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
  D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, nullptr, 0,
    D3D11_SDK_VERSION, &device, &featureLevel, &context);

  // Get DXGI device and adapter
  ComPtr<IDXGIDevice> dxgiDevice;
  device.As(&dxgiDevice);
  ComPtr<IDXGIAdapter> adapter;
  dxgiDevice->GetParent(__uuidof(IDXGIAdapter), &adapter);
  ComPtr<IDXGIOutput> output;
  adapter->EnumOutputs(0, &output);
  ComPtr<IDXGIOutput1> output1;
  output.As(&output1);

  // Duplicate the output (desktop)
  ComPtr<IDXGIOutputDuplication> duplication;
  output1->DuplicateOutput(device.Get(), &duplication);

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
  device->CreateTexture2D(&desc, nullptr, &stagingTexture);

  while (true) {
    // Capture next frame
    ComPtr<IDXGIResource> desktopResource;
    DXGI_OUTDUPL_FRAME_INFO frameInfo;
    HRESULT hr = duplication->AcquireNextFrame(16, &frameInfo, &desktopResource);
    if (FAILED(hr)) {
      std::cerr << "Failed to acquire frame.\n";
      break;
    }

    // Convert the acquired resource to texture
    ComPtr<ID3D11Texture2D> desktopTexture;
    desktopResource.As(&desktopTexture);

    // Copy the specific area from the full desktop texture to the staging texture
    D3D11_BOX sourceRegion;
    sourceRegion.left = x;
    sourceRegion.top = y;
    sourceRegion.right = x + width;
    sourceRegion.bottom = y + height;
    sourceRegion.front = 0;
    sourceRegion.back = 1;
    context->CopySubresourceRegion(stagingTexture.Get(), 0, 0, 0, 0, desktopTexture.Get(), 0, &sourceRegion);

    // Map the staging texture to CPU-accessible memory
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    hr = context->Map(stagingTexture.Get(), 0, D3D11_MAP_READ, 0, &mappedResource);
    if (SUCCEEDED(hr)) {
      // Access pixel data here
      auto* data = static_cast<uint8_t*>(mappedResource.pData);

      // Process the pixel data as needed (data format is B8G8R8A8)
      // Example: Access the first pixel's blue, green, red, and alpha channels
      uint8_t blue = data[0];
      uint8_t green = data[1];
      uint8_t red = data[2];
      uint8_t alpha = data[3];
      std::cout << "First pixel - B: " << (int)blue << ", G: " << (int)green
        << ", R: " << (int)red << ", A: " << (int)alpha << "\n";

      // Unmap the resource
      context->Unmap(stagingTexture.Get(), 0);
    }

    // Release the frame for the next capture
    duplication->ReleaseFrame();
  }
}

int main() {
  int x = 1;      // starting X coordinate of area
  int y = 1;      // starting Y coordinate of area
  int width = 800;  // width of capture area
  int height = 600; // height of capture area

  CaptureScreenArea(x, y, width, height);
  while(true) {}
  return 0;
}
