#include <windows.h>
#include <setupapi.h>
#include <devguid.h>
#include <iostream>
#include <string>
#include <vector>

#pragma comment(lib, "setupapi.lib")

const int MAX_DEVICE_ID_LEN = 999;

void ListDeviceIoctlPaths() {
  HDEVINFO deviceInfoSet = SetupDiGetClassDevs(nullptr, nullptr, nullptr, DIGCF_ALLCLASSES | DIGCF_PRESENT);
  deviceInfoSet != INVALID_HANDLE_VALUE ? deviceInfoSet : throw deviceInfoSet;

  SP_DEVINFO_DATA deviceInfoData;
  deviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
  DWORD deviceIndex = 0;

  while (SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, &deviceInfoData)) {
    TCHAR deviceInstanceId[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceInstanceId(deviceInfoSet, &deviceInfoData, deviceInstanceId, sizeof(deviceInstanceId) / sizeof(TCHAR), nullptr)) {
      std::wcout << L"Device Instance ID: " << deviceInstanceId << std::endl;
    }

    // Get the device description
    TCHAR deviceDescription[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, &deviceInfoData, SPDRP_DEVICEDESC, nullptr, (PBYTE)deviceDescription, sizeof(deviceDescription), nullptr)) {
      std::wcout << L"Device Description: " << deviceDescription << std::endl;
    }

    // Get the hardware ID
    TCHAR hardwareId[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, &deviceInfoData, SPDRP_HARDWAREID, nullptr, (PBYTE)hardwareId, sizeof(hardwareId), nullptr)) {
      std::wcout << L"Hardware ID: " << hardwareId << std::endl;
    }

    // Get other properties as needed (e.g., driver version, manufacturer)
    TCHAR manufacturer[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, &deviceInfoData, SPDRP_MFG, nullptr, (PBYTE)manufacturer, sizeof(manufacturer), nullptr)) {
      std::wcout << L"Manufacturer: " << manufacturer << std::endl;
    }

    TCHAR abc[MAX_DEVICE_ID_LEN];
    if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, &deviceInfoData, SPDRP_ADDRESS, nullptr, (PBYTE)abc, sizeof(abc), nullptr)) {
      std::wcout << L"Abc: " << abc << std::endl;
    }

    // Add more properties if needed

    std::wcout << std::endl;  // Add a newline for better readability
    deviceIndex++;
  }

  SetupDiDestroyDeviceInfoList(deviceInfoSet);
}
