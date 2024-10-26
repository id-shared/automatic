#include <windows.h>
#include <setupapi.h>
#include <devguid.h>
#include <iostream>
#include <string>
#include <vector>

#pragma comment(lib, "setupapi.lib")

#ifndef MAX_DEVICE_ID_LEN
#define MAX_DEVICE_ID_LEN 200
#endif

void ListDeviceIoctlPaths() {
  // Initialize the device information set
  HDEVINFO deviceInfoSet = SetupDiGetClassDevs(nullptr, nullptr, nullptr, DIGCF_ALLCLASSES | DIGCF_PRESENT);
  if (deviceInfoSet == INVALID_HANDLE_VALUE) {
    std::cerr << "SetupDiGetClassDevs failed with error: " << GetLastError() << std::endl;
    return;
  }

  SP_DEVINFO_DATA deviceInfoData;
  deviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
  DWORD deviceIndex = 0;

  while (SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, &deviceInfoData)) {
    // Get the device path
    TCHAR deviceInstanceId[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceInstanceId(deviceInfoSet, &deviceInfoData, deviceInstanceId, sizeof(deviceInstanceId) / sizeof(TCHAR), nullptr)) {
      std::wcout << L"Device Instance ID: " << deviceInstanceId << std::endl;

      // Optionally, retrieve IOCTLs or other properties here using CreateFile and DeviceIoControl
      // Example:
      HANDLE deviceHandle = CreateFile(deviceInstanceId, GENERIC_READ | GENERIC_WRITE,
        FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr, OPEN_EXISTING, 0, nullptr);

      if (deviceHandle != INVALID_HANDLE_VALUE) {
        // Device is open; you can now send IOCTLs as needed
        std::wcout << L"Opened device: " << deviceInstanceId << std::endl;
        // Close the handle when done
        CloseHandle(deviceHandle);
      }
      else {
        //throw deviceHandle;
      }
    }

    deviceIndex++;
  }

  SetupDiDestroyDeviceInfoList(deviceInfoSet);
}
