#pragma once
#include "Parallel.hpp"
#include "Time.hpp"
#include <d3d11.h>
#include <dxgi1_2.h>
#include <wrl.h>

namespace Capture {
  using Microsoft::WRL::ComPtr;

  bool screen(std::function<bool(uint8_t*, UINT, UINT, UINT)> z, UINT y, UINT x, UINT height, UINT width, double e) {
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
        pool.enqueue_task([&z, &height, &width, &mappedResource]() mutable {
          z((uint8_t*)mappedResource.pData, height, width, mappedResource.RowPitch);
          });
        context->Unmap(stagingTexture.Get(), 0);
      }

      duplication->ReleaseFrame();

      Time::XO(wait);
      wait = frame_time;
    }

    return true;
  };
}
