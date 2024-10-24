#include <windows.h>
#include <setupapi.h>
#include <winusb.h>
#include <hidsdi.h>
#include <iostream>
#include <string>

#pragma comment(lib, "setupapi.lib")
#pragma comment(lib, "winusb.lib")
#pragma comment(lib, "hid.lib")

// Replace these with your device's VID and PID
const int TARGET_VID = 0x046D; // Your VID
const int TARGET_PID = 0xC547; // Your PID

void ProcessMouseData(BYTE* data, int length) {
  if (length >= 3) {
    int mouseX = (int)(signed char)data[1];
    int mouseY = (int)(signed char)data[2];
    std::cout << "Mouse moved: X=" << mouseX << ", Y=" << mouseY << std::endl;
  }
}

void ReadFromDevice(WINUSB_INTERFACE_HANDLE hWinUSB) {
  BYTE buffer[64]; // Buffer size for HID reports
  DWORD bytesRead;

  while (true) {
    if (WinUsb_ReadPipe(hWinUSB, 0x81, buffer, sizeof(buffer), &bytesRead, nullptr)) {
      ProcessMouseData(buffer, bytesRead);
    }
    else {
      std::cerr << "Error reading from device. Error Code: " << GetLastError() << std::endl;
    }

    Sleep(10); // Adjust polling interval as necessary
  }
}

bool IsMatchingDevice(const std::wstring& deviceId) {
  return deviceId.find(L"VID_" + std::to_wstring(TARGET_VID)) != std::wstring::npos &&
    deviceId.find(L"PID_" + std::to_wstring(TARGET_PID)) != std::wstring::npos;
}

int main() {
  GUID InterfaceGuid;
  HidD_GetHidGuid(&InterfaceGuid);

  HDEVINFO hDevInfo = SetupDiGetClassDevs(&InterfaceGuid, nullptr, nullptr, DIGCF_PRESENT | DIGCF_INTERFACEDEVICE);
  if (hDevInfo == INVALID_HANDLE_VALUE) {
    std::cerr << "Failed to get device information set. Error Code: " << GetLastError() << std::endl;
    return 1;
  }

  SP_DEVICE_INTERFACE_DATA DeviceInterfaceData;
  DeviceInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

  for (DWORD i = 0; SetupDiEnumDeviceInterfaces(hDevInfo, nullptr, &InterfaceGuid, i, &DeviceInterfaceData); i++) {
    DWORD requiredSize;
    SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, nullptr, 0, &requiredSize, nullptr);
    PSP_DEVICE_INTERFACE_DETAIL_DATA pInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(requiredSize);
    pInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

    if (!SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, pInterfaceDetailData, requiredSize, nullptr, nullptr)) {
      std::cerr << "Failed to get device interface detail. Error Code: " << GetLastError() << std::endl;
      free(pInterfaceDetailData);
      continue;
    }

    // Retrieve the device info
    SP_DEVINFO_DATA DeviceInfoData;
    DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);

    // Now get the device instance ID using the interface detail
    WCHAR deviceId[MAX_PATH];
    if (SetupDiEnumDeviceInfo(hDevInfo, i, &DeviceInfoData) &&
      SetupDiGetDeviceInstanceId(hDevInfo, &DeviceInfoData, deviceId, sizeof(deviceId) / sizeof(WCHAR), nullptr)) {
      std::wcout << L"Device ID: " << deviceId << std::endl; // Debug output

      // Check if the device matches the target VID and PID
      if (!IsMatchingDevice(deviceId)) {
        free(pInterfaceDetailData);
        continue; // Skip to the next device if it doesn't match
      }
    }

    std::wcout << L"Matching device found: " << pInterfaceDetailData->DevicePath << std::endl;

    HANDLE hDevice = CreateFile(pInterfaceDetailData->DevicePath, GENERIC_READ | GENERIC_WRITE,
      FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr, OPEN_EXISTING, 0, nullptr);

    if (hDevice == INVALID_HANDLE_VALUE) {
      std::cerr << "Failed to open device. Error Code: " << GetLastError() << std::endl;
      free(pInterfaceDetailData);
      continue; // Skip to the next device
    }

    WINUSB_INTERFACE_HANDLE hWinUSB;
    if (!WinUsb_Initialize(hDevice, &hWinUSB)) {
      std::cerr << "Failed to initialize WinUSB. Error Code: " << GetLastError() << std::endl;
      CloseHandle(hDevice);
      free(pInterfaceDetailData);
      continue; // Skip to the next device
    }

    // Start reading from the device
    ReadFromDevice(hWinUSB);
    WinUsb_Free(hWinUSB);
    CloseHandle(hDevice);
    free(pInterfaceDetailData);
    break; // Exit after the first successful device for testing
  }

  SetupDiDestroyDeviceInfoList(hDevInfo);

  // Wait for user input to exit
  std::cout << "Hello" << std::endl;
  std::cin.get();

  return 0;
}
