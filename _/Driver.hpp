#pragma once

#include <windows.h>
#include <setupapi.h>
#include <devguid.h>
#include <iostream>
#include <string>
#include <combaseapi.h>

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
    SetupDiGetDeviceInstanceId(deviceInfoSet, &deviceInfoData, deviceInstanceId, sizeof(deviceInstanceId) / sizeof(TCHAR), nullptr);

    if (deviceInstanceId[0] == L"R"[0] && deviceInstanceId[1] == L"Z"[0] && deviceInstanceId[2] == L"C"[0]) {
      //std::cout << deviceInfoData.ClassGuid << std::endl;
      GUID classGuid = deviceInfoData.ClassGuid;
      WCHAR guidString[39]; // GUID string format is "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}"

      if (StringFromGUID2(classGuid, guidString, sizeof(guidString) / sizeof(WCHAR))) {
        std::wcout << L"Found Device GUID: " << guidString << classGuid.Data1 << std::endl;
      }
      else {
        std::cerr << "Failed to convert GUID to string." << std::endl;
      }
    }
    deviceIndex++;
  }

  SetupDiDestroyDeviceInfoList(deviceInfoSet);
}
