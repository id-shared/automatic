#include "Contact.hpp"
#include "Device.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <algorithm>
#include <fstream>
#include <iostream>
#include <vector>
#include <windows.h>

HDC hScreenDC = GetDC(NULL);
HDC hMemoryDC = CreateCompatibleDC(hScreenDC);
HBITMAP hBitmap = nullptr;

BITMAPINFO bi;
std::vector<COLORREF> pixelData;

void saveBitmap(HBITMAP hBitmap, int width, int height, const char* filePath) {
  BITMAPFILEHEADER bmfHeader;
  BITMAPINFOHEADER biHeader;
  DWORD dwBmpSize;

  biHeader.biSize = sizeof(BITMAPINFOHEADER);
  biHeader.biWidth = width;
  biHeader.biHeight = -height;
  biHeader.biPlanes = 1;
  biHeader.biBitCount = 32;
  biHeader.biCompression = BI_RGB;
  biHeader.biSizeImage = 0;
  biHeader.biXPelsPerMeter = 0;
  biHeader.biYPelsPerMeter = 0;
  biHeader.biClrUsed = 0;
  biHeader.biClrImportant = 0;

  dwBmpSize = ((width * biHeader.biBitCount + 31) / 32) * 4 * height;
  std::vector<BYTE> bmpData(dwBmpSize);

  GetDIBits(hMemoryDC, hBitmap, 0, height, bmpData.data(), reinterpret_cast<BITMAPINFO*>(&biHeader), DIB_RGB_COLORS);

  bmfHeader.bfType = 0x4D42; // BM
  bmfHeader.bfSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + dwBmpSize;
  bmfHeader.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
  bmfHeader.bfReserved1 = 0;
  bmfHeader.bfReserved2 = 0;

  std::ofstream file(filePath, std::ios::out | std::ios::binary);
  file.write(reinterpret_cast<const char*>(&bmfHeader), sizeof(bmfHeader));
  file.write(reinterpret_cast<const char*>(&biHeader), sizeof(biHeader));
  file.write(reinterpret_cast<const char*>(bmpData.data()), dwBmpSize);
  file.close();
}

bool isPurpleDominated(COLORREF color, double threshold) {
  BYTE red = GetRValue(color);
  BYTE green = GetGValue(color);
  BYTE blue = GetBValue(color);

  // Improved logic for purple dominance check
  return (red > threshold * green) && (red > threshold * blue);
}

void initCapture(int width, int height) {
  hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
  SelectObject(hMemoryDC, hBitmap);

  memset(&bi, 0, sizeof(bi));
  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = width;
  bi.bmiHeader.biHeight = -height; // Negative for top-down DIB
  bi.bmiHeader.biPlanes = 1;
  bi.bmiHeader.biBitCount = 32; // 32 bits for better color depth
  bi.bmiHeader.biCompression = BI_RGB;

  pixelData.resize(width * height);
}

void releaseCapture() {
  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);
}

std::vector<COLORREF>& capture(int startX, int startY, int width, int height) {
  // Perform BitBlt to capture the screen
  BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, startX, startY, SRCCOPY);
  // Retrieve the pixel data
  GetDIBits(hMemoryDC, hBitmap, 0, height, pixelData.data(), &bi, DIB_RGB_COLORS);
  return pixelData;
}

int findLastTrueIndex(const bool* arr, int size) {
  for (int i = size - 1; i >= 0; --i) {
    if (arr[i]) {
      return i;
    }
  }
  return -1;
}

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  const int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  const int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  const int width = 64;
  const int height = 64;
  const int delta = width / 2;

  const int startWidth = (screenWidth - width) / 2;
  const int startHeight = screenHeight / 2;

  initCapture(width, height);

  while (true) {
    auto& pixelData = capture(startWidth, startHeight, width, height);
    bool r[delta] = {};
    bool l[delta] = {};
    bool active = true;

#pragma omp parallel for
    for (int i = 0; i < height; ++i) {
      for (int j = 0; j < width; ++j) {
        COLORREF color = pixelData[(i * width) + j];
        bool isDominated = isPurpleDominated(color, 1.1);

        if (isDominated) {
          if (j < (width / 2)) {
            l[delta - j - 1] = true;
          }
          else {
            r[j - delta] = true;
          }
        }
      }
    }

    int xr = findLastTrueIndex(r, delta);
    int xl = findLastTrueIndex(l, delta);

    if (xl == -1 && xr == -1) {
      // No action needed
    }
    else if (xl == -1) {
      printf("%d\n", true);
      Xyloid2::yx(driver, 0, ((xr + 1) / 2) * +1);
    }
    else if (xr == -1) {
      printf("%d\n", true);
      Xyloid2::yx(driver, 0, ((xl + 1) / 2) * -1);
    }
    else {
      printf("%d %d\n", xl, xr);
    }

    saveBitmap(hBitmap, width, height, "e.bmp");
    // Time::XO(100); // Control the capture frequency
  }

  releaseCapture();
  return 0;
}
