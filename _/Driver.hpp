#include <windows.h>
#include <winternl.h>
#include <string>
#include <functional>
#include <iostream>
#include <cstdint>
#include <mutex>

#pragma comment(lib, "ntdll.lib")

// Define NTSTATUS manually if not included properly
typedef LONG NTSTATUS; // NTSTATUS is typically defined as LONG

extern "C" {
  constexpr NTSTATUS STATUS_SUCCESS = 0x00000000;
  constexpr NTSTATUS STATUS_MORE_ENTRIES = 0x00000105;
  constexpr NTSTATUS STATUS_BUFFER_TOO_SMALL = 0xC0000023;
  constexpr ACCESS_MASK DIRECTORY_QUERY = 0x0001;

  NTSTATUS WINAPI NtOpenDirectoryObject(
    _Out_ PHANDLE            DirectoryHandle,
    _In_  ACCESS_MASK        DesiredAccess,
    _In_  POBJECT_ATTRIBUTES ObjectAttributes
  );

  typedef struct _OBJECT_DIRECTORY_INFORMATION {
    UNICODE_STRING Name;
    UNICODE_STRING TypeName;
  } OBJECT_DIRECTORY_INFORMATION, * POBJECT_DIRECTORY_INFORMATION;

  NTSTATUS WINAPI NtQueryDirectoryObject(
    _In_      HANDLE  DirectoryHandle,
    _Out_opt_ PVOID   Buffer,
    _In_      ULONG   Length,
    _In_      BOOLEAN ReturnSingleEntry,
    _In_      BOOLEAN RestartScan,
    _Inout_   PULONG  Context,
    _Out_opt_ PULONG  ReturnLength
  );
}


inline std::wstring find_device(std::function<bool(std::wstring_view name)> p) {
  std::wstring result{};
  HANDLE dir_handle;

  OBJECT_ATTRIBUTES obj_attr;
  UNICODE_STRING obj_name;  //or RTL_CONSTANT_STRING
  RtlInitUnicodeString(&obj_name, LR"(\GLOBAL??)");
  InitializeObjectAttributes(&obj_attr, &obj_name, 0, NULL, NULL);

  if (NT_SUCCESS(NtOpenDirectoryObject(&dir_handle, DIRECTORY_QUERY, &obj_attr))) {  //or DIRECTORY_TRAVERSE?
    union {
      uint8_t buf[2048];  //#TODO
      OBJECT_DIRECTORY_INFORMATION info[1];
    };
    ULONG context;

#pragma warning(suppress : 6001)  //Warning C6001: Using uninitialized memory 'context'.
    NTSTATUS status = NtQueryDirectoryObject(dir_handle, buf, sizeof buf, false, true, &context, NULL);
    while (NT_SUCCESS(status)) {  //STATUS_SUCCESS, STATUS_MORE_ENTRIES
      bool found = false;
      for (ULONG i = 0; info[i].Name.Buffer; i++) {
        std::wstring_view sv{ info[i].Name.Buffer, info[i].Name.Length / sizeof(wchar_t) };
        if (p(sv)) {
          result = LR"(\??\)" + std::wstring(sv);
          found = true;
          break;
        }
      }
      if (found || status != STATUS_MORE_ENTRIES)
        break;
      status = NtQueryDirectoryObject(dir_handle, buf, sizeof buf, false, false, &context, NULL);
    }

    CloseHandle(dir_handle);
  }

  return result;
}
