#pragma once

#include <windows.h>
#include <setupapi.h>
#include <devguid.h>
#include <iostream>
#include <string>
#include <vector>

#pragma comment(lib, "setupapi.lib")

const int MAX_DEVICE_ID_LEN = 999;

std::vector<DWORD> spdrpConstants = {
    SPDRP_DEVICEDESC                  ,
    SPDRP_HARDWAREID                  ,
    SPDRP_COMPATIBLEIDS               ,
    SPDRP_UNUSED0                     ,
    SPDRP_SERVICE                     ,
    SPDRP_UNUSED1                     ,
    SPDRP_UNUSED2                     ,
    SPDRP_CLASS                       ,
    SPDRP_CLASSGUID                   ,
    SPDRP_DRIVER                      ,
    SPDRP_CONFIGFLAGS                 ,
    SPDRP_MFG                         ,
    SPDRP_FRIENDLYNAME                ,
    SPDRP_LOCATION_INFORMATION        ,
    SPDRP_PHYSICAL_DEVICE_OBJECT_NAME ,
    SPDRP_CAPABILITIES                ,
    SPDRP_UI_NUMBER                   ,
    SPDRP_UPPERFILTERS                ,
    SPDRP_LOWERFILTERS                ,
    SPDRP_BUSTYPEGUID                 ,
    SPDRP_LEGACYBUSTYPE               ,
    SPDRP_BUSNUMBER                   ,
    SPDRP_ENUMERATOR_NAME             ,
    SPDRP_SECURITY                    ,
    SPDRP_SECURITY_SDS                ,
    SPDRP_DEVTYPE                     ,
    SPDRP_EXCLUSIVE                   ,
    SPDRP_CHARACTERISTICS             ,
    SPDRP_ADDRESS                     ,
    SPDRP_UI_NUMBER_DESC_FORMAT       ,
    SPDRP_DEVICE_POWER_DATA           ,
    SPDRP_REMOVAL_POLICY              ,
    SPDRP_REMOVAL_POLICY_HW_DEFAULT   ,
    SPDRP_REMOVAL_POLICY_OVERRIDE     ,
    SPDRP_INSTALL_STATE               ,
    SPDRP_LOCATION_PATHS              ,
    SPDRP_BASE_CONTAINERID            ,
    SPDRP_MAXIMUM_PROPERTY            ,
    SPCRP_UPPERFILTERS                ,
    SPCRP_LOWERFILTERS                ,
    SPCRP_SECURITY                    ,
    SPCRP_SECURITY_SDS                ,
    SPCRP_DEVTYPE                     ,
    SPCRP_EXCLUSIVE                   ,
    SPCRP_CHARACTERISTICS             ,
    SPCRP_MAXIMUM_PROPERTY            ,
};

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
