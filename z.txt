#include <windows.h>
#include <iostream>

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
  switch (uMsg) {
  case WM_INPUT: {
    UINT dwSize;
    GetRawInputData((HRAWINPUT)lParam, RID_INPUT, nullptr, &dwSize, sizeof(RAWINPUTHEADER));
    LPBYTE lpb = new BYTE[dwSize];
    if (lpb == nullptr) return 0;

    if (GetRawInputData((HRAWINPUT)lParam, RID_INPUT, lpb, &dwSize, sizeof(RAWINPUTHEADER)) != dwSize) {
      std::cerr << "Error getting raw input data." << std::endl;
    }

    RAWINPUT* rawInput = (RAWINPUT*)lpb;
    if (rawInput->header.dwType == RIM_TYPEMOUSE) {
      // Process mouse movement data
      int mouseX = rawInput->data.mouse.lLastX;
      int mouseY = rawInput->data.mouse.lLastY;
      std::cout << "Mouse moved: X=" << mouseX << ", Y=" << mouseY << std::endl;
    }

    delete[] lpb;
    return 0;
  }
  case WM_DESTROY:
    PostQuitMessage(0);
    return 0;
  }

  return DefWindowProc(hwnd, uMsg, wParam, lParam);
}

int main() {
  // Register the window class
  const wchar_t CLASS_NAME[] = L"RawInputWindowClass"; // Use wide-character literal
  WNDCLASS wc = {};
  wc.lpfnWndProc = WindowProc;
  wc.hInstance = GetModuleHandle(nullptr);
  wc.lpszClassName = CLASS_NAME;

  RegisterClass(&wc);

  // Create the window
  HWND hwnd = CreateWindowEx(
    0, CLASS_NAME, L"HID Mouse Movement Detector", WS_OVERLAPPEDWINDOW, // Use wide-character literal
    CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
    nullptr, nullptr, GetModuleHandle(nullptr), nullptr
  );

  // Register for raw input
  RAWINPUTDEVICE rid;
  rid.usUsagePage = 0x01;  // Generic desktop controls
  rid.usUsage = 0x02;       // Mouse
  rid.dwFlags = RIDEV_INPUTSINK; // Receive input even when not in the foreground
  rid.hwndTarget = hwnd;

  if (RegisterRawInputDevices(&rid, 1, sizeof(rid)) == -1) {
    std::cerr << "Failed to register raw input devices." << std::endl;
    return 1;
  }

  ShowWindow(hwnd, SW_HIDE); // Hide the window

  // Message loop
  MSG msg;
  while (GetMessage(&msg, nullptr, 0, 0)) {
    TranslateMessage(&msg);
    DispatchMessage(&msg);
  }

  return 0;
}
