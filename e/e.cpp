#include "Contact.hpp"
#include "Device.hpp"
#include "Ram.hpp"
#include "Xyloid2.hpp"
#include <chrono>
#include <condition_variable>
#include <d3d11.h>
#include <dxgi1_2.h>
#include <functional>
#include <iostream>
#include <mutex>
#include <queue>
#include <thread>
#include <wrl.h>

using Microsoft::WRL::ComPtr;

struct FrameData {
  std::vector<uint8_t> data;
  UINT row_pitch;
};

std::queue<FrameData> frameQueue;
std::mutex queueMutex;
std::condition_variable queueCV;
bool stopProcessing = false;

void ProcessFrame(FrameData frame, std::function<bool(uint8_t*, int)> processPixelData) {
  processPixelData(frame.data.data(), frame.row_pitch);
}

void ProcessFrames(std::function<bool(uint8_t*, int)> processPixelData, int numThreads) {
  std::vector<std::thread> workers;

  for (int i = 0; i < numThreads; ++i) {
    workers.emplace_back([&]() {
      while (!stopProcessing) {
        FrameData frame;
        {
          std::unique_lock<std::mutex> lock(queueMutex);
          queueCV.wait(lock, [] { return !frameQueue.empty() || stopProcessing; });

          if (stopProcessing) break;

          frame = std::move(frameQueue.front());
          frameQueue.pop();
        }

        ProcessFrame(frame, processPixelData);
      }
      });
  }

  for (auto& worker : workers) {
    worker.join();
  }
}

void CaptureScreenArea(std::function<bool(uint8_t*, int)> processPixelData, int frame_time, int x, int y, int width, int height) {
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

  while (!stopProcessing) {
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
      std::vector<uint8_t> frameData((uint8_t*)mappedResource.pData, (uint8_t*)mappedResource.pData + mappedResource.RowPitch * height);

      {
        std::lock_guard<std::mutex> lock(queueMutex);
        frameQueue.emplace(FrameData{ std::move(frameData), mappedResource.RowPitch });
      }
      queueCV.notify_one();
      context->Unmap(stagingTexture.Get(), 0);
    }

    duplication->ReleaseFrame();
  }
}

bool isKeyHeld(int e) {
  return (GetAsyncKeyState(e) & 0x8000) != +0;
}

int speed(int e) {
  int ae = std::abs(e);
  return (ae > +4) ? (ae % +2 == +1 ? +2 : +1) : +1;
}

int main() {
  const int count = std::thread::hardware_concurrency();
  const int wide = +16 * +5;
  const int high = +16 * +1;
  const int each = +1;

  const int xy = (1080 - high) / +2;
  const int xx = (1920 - wide) / +2;

  const int ey = high / +2;
  const int ex = wide / +2;

  const int cy = +4;
  const int cx = +2;

  int ay = +1;
  int ax = +1;

  bool _r = false;
  bool _l = false;

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);
  std::function<bool()> lambda = [&_l, each]() {
    while (true) {
      _l = isKeyHeld(VK_LBUTTON);
      std::this_thread::sleep_for(std::chrono::milliseconds(each));
    }
    return true;
    };

  std::thread t(lambda);

  std::function<bool(uint8_t*, int)> process = [&ax, &ay, &_l, driver](uint8_t* _o, UINT row_pitch) {
    for (int y = 0; y < high; ++y) {
      uint8_t* row_ptr = _o + y * row_pitch;

      for (int x = 0; x < wide; ++x) {
        uint8_t* pixel = row_ptr + x * 4;

        if (pixel[0] >= 251 && pixel[1] <= 191 && pixel[2] >= 251 && pixel[3] == 255) {
          ay = (y - ey) + cy;

          ax = (x - ex) + cx;

          return Xyloid2::yx(driver, _l ? +0 : ay * speed(ay), ax * speed(ax));
        }
      }
    }

    return true;
    };

  std::thread captureThread(CaptureScreenArea, process, each, xx, xy, wide, high);
  std::thread processThread(ProcessFrames, process, count);

  captureThread.join();
  stopProcessing = true;
  queueCV.notify_all();
  processThread.join();

  return 0;
}
