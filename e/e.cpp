#include "Contact.hpp"
#include "Device.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <algorithm>
#include <iostream>
#include <vector>
#include <windows.h>

HDC hScreenDC = GetDC(NULL);
HDC hMemoryDC = CreateCompatibleDC(hScreenDC);
HBITMAP hBitmap = nullptr;

BITMAPINFO bi;
std::vector<COLORREF> pixelData;

void initCapture(int e_1, int e) {
  hBitmap = CreateCompatibleBitmap(hScreenDC, e_1, e);
  SelectObject(hMemoryDC, hBitmap);

  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = e_1;
  bi.bmiHeader.biHeight = e;
  bi.bmiHeader.biPlanes = 1;
  bi.bmiHeader.biBitCount = 32;
  bi.bmiHeader.biCompression = BI_RGB;
  bi.bmiHeader.biSizeImage = 0;
  bi.bmiHeader.biXPelsPerMeter = 0;
  bi.bmiHeader.biYPelsPerMeter = 0;
  bi.bmiHeader.biClrUsed = 0;
  bi.bmiHeader.biClrImportant = 0;

  pixelData.resize(e_1 * e);
}

void releaseCapture() {
  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);
}

std::vector<COLORREF>& capture(int e_1, int e) {
  int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  int x = (screenWidth - e_1) / 2;
  int y = (screenHeight - e) / 2;

  BitBlt(hMemoryDC, 0, 0, e_1, e, hScreenDC, x, y, SRCCOPY);
  GetDIBits(hMemoryDC, hBitmap, 0, e, pixelData.data(), &bi, DIB_RGB_COLORS);

  return pixelData;
}

bool IsPurpleDominated(COLORREF x, double e) {
  BYTE green = GetGValue(x);
  BYTE blue = GetBValue(x);
  BYTE red = GetRValue(x);

  if (green == 0) {
    return (red > 0 && blue > 0);
  }

  double ratio_red_green = static_cast<double>(red) / green;
  double ratio_blue_green = static_cast<double>(blue) / green;

  return (ratio_red_green > e && ratio_blue_green > e);
}

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);
  const int width = 64;
  const int height = 4;
  const int delta = (width / 2);

  initCapture(width, height);

  while (true) {
    auto& pixelData = capture(width, height);
    bool r[delta] = {};
    bool l[delta] = {};
    bool active = true;

#pragma omp parallel for
    for (int i = 0; i < height; ++i) {
      for (int j = 0; j < width; ++j) {
        COLORREF color = pixelData[(i * width) + j];
        bool result = IsPurpleDominated(color, 1.2);
        j < delta ? l[(delta - 1) - j] = result : r[j - delta] = result;
      }
    }

    for (int i = 0; i < delta && active; ++i) {
      if (r[i]) {
        Xyloid2::yx(driver, 0, (i + 1));
        active = false;
      }
    }

    for (int i = 0; i < delta && active; ++i) {
      if (l[i]) {
        Xyloid2::yx(driver, 0, -(i + 1));
        active = false;
      }
    }
  }

  releaseCapture();

  return 0;
}
