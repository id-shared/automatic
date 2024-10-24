#include <windows.h>
#include <setupapi.h>
#include <hidsdi.h>
#include <iostream>
#include <string>

#pragma comment(lib, "setupapi.lib")
#pragma comment(lib, "hid.lib")

void ProcessMouseData(BYTE* data, int length) {
  if (length >= 3) {
    int mouseX = (int)(signed char)data[1];
    int mouseY = (int)(signed char)data[2];
    std::cout << "Mouse moved: X=" << mouseX << ", Y=" << mouseY << std::endl;
  }
}

void ReadFromDevice(HANDLE hDevice) {
  BYTE buffer[64]; // Buffer size for HID reports

  while (true) {
    if (HidD_GetInputReport(hDevice, buffer, sizeof(buffer))) {
      ProcessMouseData(buffer, sizeof(buffer));
    }
    else {
      std::cerr << "Failed to read input report. Error Code: " << GetLastError() << std::endl;
    }
    Sleep(10); // Adjust polling interval as necessary
  }
}

bool IsMatchingDevice(const std::wstring& deviceId) {
  // Check if the device ID matches the target VID and PID
  return deviceId.find(L"HID\\VID_046D&PID_C547") == 0; // Replace with your mouse's VID and PID
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

    WCHAR deviceId[MAX_PATH];
    if (SetupDiEnumDeviceInfo(hDevInfo, i, &DeviceInfoData) &&
      SetupDiGetDeviceInstanceId(hDevInfo, &DeviceInfoData, deviceId, sizeof(deviceId) / sizeof(WCHAR), nullptr)) {
      std::wcout << L"Device ID: " << deviceId << std::endl; // Debug output

      // Check if the device matches the target VID
      if (!IsMatchingDevice(deviceId)) {
        free(pInterfaceDetailData);
        continue; // Skip to the next device if it doesn't match
      }
    }

    std::wcout << L"Matching device found: " << pInterfaceDetailData->DevicePath << std::endl;

    HANDLE hDevice = CreateFileW(
      pInterfaceDetailData->DevicePath,
      GENERIC_READ | GENERIC_WRITE,
      FILE_SHARE_READ | FILE_SHARE_WRITE,
      nullptr,
      OPEN_EXISTING,
      0,
      nullptr
    );

    if (hDevice == INVALID_HANDLE_VALUE) {
      DWORD dwError = GetLastError();
      std::cerr << "Failed to open device. Error Code: " << dwError << std::endl;

      // Log additional information based on the error code
      if (dwError == ERROR_ACCESS_DENIED) {
        std::cerr << "Access denied. Try running the program as an administrator." << std::endl;
      }
      else if (dwError == ERROR_FILE_NOT_FOUND) {
        std::cerr << "The specified device was not found." << std::endl;
      }
      else {
        std::cerr << "Error Code Description: " << std::system_category().message(dwError) << std::endl;
      }
      free(pInterfaceDetailData);
      continue; // Skip to the next device
    }

    std::wcout << L"Device opened successfully: " << pInterfaceDetailData->DevicePath << std::endl;

    // Start reading from the device
    ReadFromDevice(hDevice);

    CloseHandle(hDevice);
    free(pInterfaceDetailData);
    break; // Exit after the first successful device for testing
  }

  SetupDiDestroyDeviceInfoList(hDevInfo);

  // Wait for user input to exit
  std::cout << "Press Enter to exit." << std::endl;
  std::cin.get();

  return 0;
}
