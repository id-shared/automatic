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

void initCapture(int e_1, int e) {
  hBitmap = CreateCompatibleBitmap(hScreenDC, e_1, e);
  SelectObject(hMemoryDC, hBitmap);

  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = e_1;
  bi.bmiHeader.biHeight = -e;
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

void saveBitmap(HBITMAP hBitmap, int width, int height, const char* filePath) {
  BITMAPFILEHEADER bmfHeader;
  BITMAPINFOHEADER biHeader;
  DWORD dwBmpSize;

  biHeader.biSize = sizeof(BITMAPINFOHEADER);
  biHeader.biWidth = width;
  biHeader.biHeight = height;
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

std::vector<COLORREF>& capture(int e_3, int e_2, int e_1, int e) {
  BitBlt(hMemoryDC, 0, 0, e_1, e, hScreenDC, e_3, e_2, SRCCOPY);
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
  const int startHeight = (screenHeight / 2) + height;

  initCapture(width, height);

  while (true) {
    auto& pixelData = capture(startWidth, startHeight, width, height);
    bool r[delta] = {};
    bool l[delta] = {};
    bool active = true;

#pragma omp parallel for
    for (int i = 0; i < height; ++i) {
      //printf("%d, %d %d %d %d\n", i, IsPurpleDominated(pixelData[(i * width)], 1.2), IsPurpleDominated(pixelData[(i * width) + 1], 1.2), IsPurpleDominated(pixelData[(i * width) + 2], 1.2), IsPurpleDominated(pixelData[(i * width) + 3], 1.2));

      for (int j = 0; j < width; ++j) {
        COLORREF color = pixelData[(i * width) + j];
        IsPurpleDominated(color, 1.5) ? (
          j < (width / 2) ? l[delta - j - 1] = true : r[j - delta] = true
          ) : false;
      }
    }

    int xr = findLastTrueIndex(r, delta);
    int xl = findLastTrueIndex(l, delta);

    if (xl == -1 && xr == -1) {
    }
    else if (xl == -1) {
      Xyloid2::yx(driver, 0, ((xr + 1) / 2) * +1);
      /*for (int i = 0; i < delta && active; ++i) {
        if (r[i]) {
          Xyloid2::yx(driver, 0, ((xr + 1) / 2) * +1);
          active = false;
        }
      }*/
    }
    else if (xr == -1) {
      Xyloid2::yx(driver, 0, ((xl + 1) / 2) * -1);
      /*for (int i = 0; i < delta && active; ++i) {
        if (l[i]) {
          Xyloid2::yx(driver, 0, ((xl + 1) / 2) * -1);
          active = false;
        }
      }*/
    }
    else {
      printf("%d %d\n", xl, xr);
    }

    //saveBitmap(hBitmap, width, height, "e.bmp");
    //Time::XO(100);
  }

  releaseCapture();
  return 0;
}

//if (std::any_of(l, l + 16, [](bool value) { return value; }) && std::any_of(r, r + 16, [](bool value) { return value; })) {
//  std::cout << "At least one of the first three elements is true.\n";
//}
