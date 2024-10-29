#include <windows.h>
#include <iostream>

void CaptureScreen(int width, int height) {
  // Get the device context of the entire screen
  HDC hScreenDC = GetDC(NULL);
  HDC hMemoryDC = CreateCompatibleDC(hScreenDC);

  // Create a bitmap to hold the captured pixels
  HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
  SelectObject(hMemoryDC, hBitmap);

  // BitBlt (bit block transfer) from the screen to our bitmap
  int screenWidth = GetSystemMetrics(SM_CXSCREEN);
  int screenHeight = GetSystemMetrics(SM_CYSCREEN);
  int x = (screenWidth - width) / 2; // Center x
  int y = (screenHeight - height) / 2; // Center y

  BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, x, y, SRCCOPY);

  // Save the bitmap to a file
  BITMAPFILEHEADER bmfHeader;
  BITMAPINFOHEADER bi;

  bi.biSize = sizeof(BITMAPINFOHEADER);
  bi.biWidth = width;
  bi.biHeight = -height; // Negative height for a top-down DIB
  bi.biPlanes = 1;
  bi.biBitCount = 32; // 32 bits for ARGB
  bi.biCompression = BI_RGB;
  bi.biSizeImage = 0;
  bi.biXPelsPerMeter = 0;
  bi.biYPelsPerMeter = 0;
  bi.biClrUsed = 0;
  bi.biClrImportant = 0;

  // Calculate the size of the bitmap file
  DWORD dwBmpSize = ((width * bi.biBitCount + 31) / 32) * 4 * height;
  DWORD dwSizeofDIB = dwBmpSize + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

  // Create a file to save the bitmap
  HANDLE hFile = CreateFile(L"captured.bmp", GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
  if (hFile == INVALID_HANDLE_VALUE) {
    std::cerr << "Could not create file." << std::endl;
    return;
  }

  // Fill the bitmap file header
  bmfHeader.bfType = 0x4D42; // 'BM'
  bmfHeader.bfSize = dwSizeofDIB;
  bmfHeader.bfReserved1 = 0;
  bmfHeader.bfReserved2 = 0;
  bmfHeader.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

  // Write the bitmap file header and info header to the file
  DWORD dwWritten;
  WriteFile(hFile, &bmfHeader, sizeof(BITMAPFILEHEADER), &dwWritten, NULL);
  WriteFile(hFile, &bi, sizeof(BITMAPINFOHEADER), &dwWritten, NULL);

  // Write the bitmap data
  GetDIBits(hMemoryDC, hBitmap, 0, height, (LPVOID)((char*)&bmfHeader + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER)), (BITMAPINFO*)&bi, DIB_RGB_COLORS);
  WriteFile(hFile, (LPVOID)((char*)&bmfHeader + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER)), dwBmpSize, &dwWritten, NULL);

  // Cleanup
  CloseHandle(hFile);
  DeleteObject(hBitmap);
  DeleteDC(hMemoryDC);
  ReleaseDC(NULL, hScreenDC);
}

int main() {
  const int width = 10;
  const int height = 10;
  CaptureScreen(width, height);
  std::cout << "Captured " << width << "x" << height << " pixels at the center of the screen." << std::endl;
  while(true){}
  return 0;
}
