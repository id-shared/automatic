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
struct Result {
  int firstIndex; // Index of the first true value
  int lastIndex;  // Index of the last true value

  Result() : firstIndex(-1), lastIndex(-1) {} // Initialize to -1
};

Result findFirstAndLastTrue(const std::vector<bool>& arr) {
  Result result; // Create a Result object

  for (size_t i = 0; i < arr.size(); ++i) {
    if (arr[i]) {
      if (result.firstIndex == -1) {
        result.firstIndex = i; // First true found
      }
      result.lastIndex = i; // Update last true found
    }
  }

  return result; // Return the Result object
}

template <typename T>
std::vector<std::vector<T>> splitArray(const std::vector<T>& array, int n) {
  std::vector<std::vector<T>> parts;
  int partSize = array.size() / n;

  for (int i = 0; i < n; ++i) {
    auto startIt = array.begin() + i * partSize;
    auto endIt = (i == n - 1) ? array.end() : startIt + partSize;
    parts.emplace_back(startIt, endIt);
  }

  return parts;
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

  const int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  const int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  const int width = 4;
  const int height = 2;

  const int startWidth = (screenWidth - width) / 2;
  const int startHeight = (screenHeight / 2) + height;

  initCapture(width, height);

  while (true) {
    auto& pixelData = capture(startWidth, startHeight, width, height);
    bool r[width / 2] = {};
    bool l[width / 2] = {};
    bool active = true;

    std::vector<std::vector<COLORREF>> detail = splitArray(pixelData, width);

#pragma omp parallel for
    for (int i = 0; i < height; ++i) {
      for (int j = 0; j < width; ++j) {
        COLORREF color = detail[i][j];
        printf("%d %d %d %d\n", IsPurpleDominated(detail[i][0], 1.5), IsPurpleDominated(detail[i][1], 1.5), IsPurpleDominated(detail[i][2], 1.5), IsPurpleDominated(detail[i][3], 1.5));
        bool result = IsPurpleDominated(color, 1.5);
        j < (width / 2) ? l[(width / 2 - 1) - j] = result : r[j - (width / 2)] = result;
      }
    }

    for (int i = 0; i < (width / 2) && active; ++i) {
      if (r[i]) {
        Xyloid2::yx(driver, 0, +i);
        active = false;
      }
    }

    for (int i = 0; i < (width / 2) && active; ++i) {
      if (l[i]) {
        Xyloid2::yx(driver, 0, -i);
        active = false;
      }
    }

    //saveBitmap(hBitmap, width, height, "e.bmp");
    Time::XO(100);
  }

  releaseCapture();

  return 0;
}

//if (std::any_of(l, l + 16, [](bool value) { return value; }) && std::any_of(r, r + 16, [](bool value) { return value; })) {
//  std::cout << "At least one of the first three elements is true.\n";
//}
