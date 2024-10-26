#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>
#include <string>
#include <functional>
#include <iostream>
#include <cstdint>
#include <mutex>

#pragma comment(lib, "ntdll.lib")

// Make sure NTSTATUS is defined
typedef LONG NTSTATUS; // or #define NTSTATUS LONG

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
