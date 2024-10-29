#include "Contact.hpp"
#include "Device.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <iostream>
#include <vector>
#include <windows.h>

std::vector<COLORREF> capture(int e_1, int e) {
  HDC hScreenDC = GetDC(NULL);
  HDC hMemoryDC = CreateCompatibleDC(hScreenDC);

  HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC, e_1, e);
  SelectObject(hMemoryDC, hBitmap);

  int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  int x = (screenWidth - e_1) / 2;
  int y = (screenHeight - e) / 2;

  BitBlt(hMemoryDC, 0, 0, e_1, e, hScreenDC, x, y, SRCCOPY);

  BITMAP bmp;
  GetObject(hBitmap, sizeof(BITMAP), &bmp);

  BITMAPINFO bi;
  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = bmp.bmWidth;
  bi.bmiHeader.biHeight = bmp.bmHeight;
  bi.bmiHeader.biPlanes = 1;
  bi.bmiHeader.biBitCount = 32;
  bi.bmiHeader.biCompression = BI_RGB;
  bi.bmiHeader.biSizeImage = 0;
  bi.bmiHeader.biXPelsPerMeter = 0;
  bi.bmiHeader.biYPelsPerMeter = 0;
  bi.bmiHeader.biClrUsed = 0;
  bi.bmiHeader.biClrImportant = 0;

  std::vector<COLORREF> pixelData(e_1 * e);
  GetDIBits(hMemoryDC, hBitmap, 0, e, pixelData.data(), &bi, DIB_RGB_COLORS);

  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);

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

  while (true) {
    const int width = 8;
    const int height = 2;
    const int delta = (width / 2);

    std::vector<COLORREF> pixelData = capture(width, height);
    bool breaker = true;
    bool r[delta] = {};
    bool l[delta] = {};
    for (int i = 0; i < height; ++i) {
      for (int j = 0; j < width; ++j) {
        COLORREF color = pixelData[(i * width) + j];
        bool result = IsPurpleDominated(color, 1.25);
        j <= delta ? l[j] = result : r[j] = result;
      }
    }

    printf("%d, %d, %d, %d, %d, %d, %d, %d\n", l[0], l[1], l[2], l[3], r[0], r[1], r[2], r[3]);
    Time::XO(100);
  }

  return 0;
}

//if (j >= delta) {
  //Xyloid2::yx(driver, 0, (j - delta) * +1);
//}
//else {
  //Xyloid2::yx(driver, 0, ((delta + 2) - j) * -1);
//}
