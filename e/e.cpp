#include <windows.h>
#include <iostream>
#include <vector>

void CaptureScreen(int width, int height, std::vector<COLORREF>& pixelData) {
  // Get the device context of the entire screen
  HDC hScreenDC = GetDC(NULL);
  HDC hMemoryDC = CreateCompatibleDC(hScreenDC);

  // Create a bitmap to hold the captured pixels
  HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
  SelectObject(hMemoryDC, hBitmap);

  // BitBlt from the screen to our bitmap
  int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  int x = (screenWidth - width) / 2; // Center x
  int y = (screenHeight - height) / 2; // Center y

  BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, x, y, SRCCOPY);

  // Prepare to retrieve pixel data
  BITMAP bmp;
  GetObject(hBitmap, sizeof(BITMAP), &bmp);

  // Create a buffer to hold the bitmap data
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

  // Allocate memory for pixel data
  pixelData.resize(width * height);
  GetDIBits(hMemoryDC, hBitmap, 0, height, pixelData.data(), &bi, DIB_RGB_COLORS);

  // Cleanup
  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);
}

int main() {
  const int width = 10;
  const int height = 1;
  std::vector<COLORREF> pixelData;

  // Capture the screen
  CaptureScreen(width, height, pixelData);

  // Process the pixel data
  for (int i = 0; i < height; ++i) {
    for (int j = 0; j < width; ++j) {
      COLORREF color = pixelData[i * width + j];
      BYTE red = GetRValue(color);
      BYTE green = GetGValue(color);
      BYTE blue = GetBValue(color);
      // Print or process the color values (R, G, B)
      std::cout << "Pixel (" << j << ", " << i << "): R=" << static_cast<int>(red)
        << " G=" << static_cast<int>(green) << " B=" << static_cast<int>(blue) << std::endl;
    }
  }
  while(true) {}
  return 0;
}
