#include <windows.h>
#include <iostream>
#include <vector>

std::vector<COLORREF> CaptureScreen(int width, int height) {
  HDC hScreenDC = GetDC(NULL);
  HDC hMemoryDC = CreateCompatibleDC(hScreenDC);

  HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
  SelectObject(hMemoryDC, hBitmap);

  int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  int x = (screenWidth - width) / 2; // Center x
  int y = (screenHeight - height) / 2; // Center y

  BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, x, y, SRCCOPY);

  BITMAP bmp;
  GetObject(hBitmap, sizeof(BITMAP), &bmp);

  BITMAPINFO bi;
  bi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
  bi.bmiHeader.biWidth = bmp.bmWidth;
  bi.bmiHeader.biHeight = bmp.bmHeight;
  bi.bmiHeader.biPlanes = 1;
  bi.bmiHeader.biBitCount = 32; // 32 bits for ARGB
  bi.bmiHeader.biCompression = BI_RGB;
  bi.bmiHeader.biSizeImage = 0;
  bi.bmiHeader.biXPelsPerMeter = 0;
  bi.bmiHeader.biYPelsPerMeter = 0;
  bi.bmiHeader.biClrUsed = 0;
  bi.bmiHeader.biClrImportant = 0;

  std::vector<COLORREF> pixelData(width * height);
  GetDIBits(hMemoryDC, hBitmap, 0, height, pixelData.data(), &bi, DIB_RGB_COLORS);

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
  while (true) {
    const int width = 16;
    const int height = 4;

    std::vector<COLORREF> pixelData = CaptureScreen(width, height);

    for (int i = 0; i < height; ++i) {
      for (int j = 0; j < width; ++j) {
        COLORREF color = pixelData[i * width + j];
        if (IsPurpleDominated(color, 1.5)) {
          std::cout << j << "," << i << std::endl;
        }
      }
    }
  }
  return 0;
}
