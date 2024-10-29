#include "Contact.hpp"
#include "Device.hpp"
#include "Time.hpp"
#include "Xyloid2.hpp"
#include <iostream>
#include <vector>
#include <windows.h>

std::vector<COLORREF> capture(HDC hScreenDC, HDC hMemoryDC, HBITMAP hBitmap, int e_1, int e) {
  BitBlt(hMemoryDC, 0, 0, e_1, e, hScreenDC, (GetSystemMetrics(SM_CXSCREEN) - e_1) / 2,
    (GetSystemMetrics(SM_CYSCREEN) - e) / 2, SRCCOPY);

  BITMAPINFO bi;
  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = e_1;
  bi.bmiHeader.biHeight = -e;  // Top-down DIB for direct order
  bi.bmiHeader.biPlanes = 1;
  bi.bmiHeader.biBitCount = 32;
  bi.bmiHeader.biCompression = BI_RGB;

  std::vector<COLORREF> pixelData(e_1 * e);
  GetDIBits(hMemoryDC, hBitmap, 0, e, pixelData.data(), &bi, DIB_RGB_COLORS);

  return pixelData;
}

inline bool IsPurpleDominated(COLORREF x, double e) {
  BYTE green = GetGValue(x);
  if (green == 0) return (GetRValue(x) > 0 && GetBValue(x) > 0);

  double ratio = (static_cast<double>(GetRValue(x)) + GetBValue(x)) / (2 * green);
  return ratio > e;
}

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  const int width = 64, height = 4, delta = width / 2;
  HDC hScreenDC = GetDC(NULL);
  HDC hMemoryDC = CreateCompatibleDC(hScreenDC);
  HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
  SelectObject(hMemoryDC, hBitmap);

  while (true) {
    std::vector<COLORREF> pixelData = capture(hScreenDC, hMemoryDC, hBitmap, width, height);
    bool r[delta] = {}, l[delta] = {};
    bool active = true;

    for (int i = 0; i < width; ++i) {
      for (int j = 0; j < width; ++j) {
        bool result = IsPurpleDominated(pixelData[(i * width) + j], 1.2);
        j < delta ? l[j] = result : r[j - delta] = result;
      }
    }

    std::reverse(l, l + delta);

    for (int i = 0; i < delta && active; ++i) {
      if (r[i]) {
        Xyloid2::yx(driver, 0, (i + 1) * +1);
        active = false;
      }
    }

    for (int i = 0; i < delta && active; ++i) {
      if (l[i]) {
        Xyloid2::yx(driver, 0, (i + 1) * -1);
        active = false;
      }
    }
  }

  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);
  return 0;
}
