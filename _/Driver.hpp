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
    SetupDiGetDeviceInstanceId(deviceInfoSet, &deviceInfoData, deviceInstanceId, sizeof(deviceInstanceId) / sizeof(TCHAR), nullptr);

    if (deviceInstanceId == L"RZCONTROL\\VID_1532&PID_0306&MI_00\\3&2CD34B8&0") {
      TCHAR abc[MAX_DEVICE_ID_LEN];
      if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, &deviceInfoData, SPDRP_ENUMERATOR_NAME, nullptr, (PBYTE)abc, sizeof(abc), nullptr)) {
        std::wcout << L"Abc: " << deviceInstanceId << std::endl;
      }
    }

    std::wcout << std::endl;
    deviceIndex++;
  }

  SetupDiDestroyDeviceInfoList(deviceInfoSet);
}
