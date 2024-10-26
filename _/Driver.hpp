#include <windows.h>
#include <winternl.h>
#include <string>
#include <functional>
#include <iostream>
#include <cstdint>
#include <mutex>
#include <vector>

#pragma comment(lib, "ntdll.lib")

typedef LONG NTSTATUS;

extern "C" {
  constexpr NTSTATUS STATUS_SUCCESS = 0x00000000;
  constexpr NTSTATUS STATUS_MORE_ENTRIES = 0x00000105;
  constexpr NTSTATUS STATUS_BUFFER_TOO_SMALL = 0xC0000023;
  constexpr ACCESS_MASK DIRECTORY_QUERY = 0x0001;
  constexpr ACCESS_MASK DIRECTORY_TRAVERSE = 0x0002;

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

inline const wchar_t* find_device(std::function<bool(std::wstring_view name)> predicate) {
  OBJECT_ATTRIBUTES obj_attr;
  UNICODE_STRING obj_name;
  HANDLE dir_handle;
  static std::wstring result{};  // Static to persist after function scope

  RtlInitUnicodeString(&obj_name, L"\\GLOBAL??");
  InitializeObjectAttributes(&obj_attr, &obj_name, 0, NULL, NULL);

  if (NT_SUCCESS(NtOpenDirectoryObject(&dir_handle, DIRECTORY_QUERY | DIRECTORY_TRAVERSE, &obj_attr))) {
    const size_t buffer_size = 2048;
    std::vector<unsigned char> buf(buffer_size);
    ULONG context = 0;
    NTSTATUS status = NtQueryDirectoryObject(dir_handle, buf.data(), static_cast<ULONG>(buf.size()), FALSE, TRUE, &context, NULL);

    while (NT_SUCCESS(status)) {
      bool found = false;

      POBJECT_DIRECTORY_INFORMATION info = reinterpret_cast<POBJECT_DIRECTORY_INFORMATION>(buf.data());
      for (ULONG i = 0; info[i].Name.Buffer; i++) {
        std::wstring_view sv{ info[i].Name.Buffer, info[i].Name.Length / sizeof(wchar_t) };
        if (predicate(sv)) {
          result = LR"(\??\)" + std::wstring(sv);
          found = true;
          break;
        }
      }
      if (found || status != STATUS_MORE_ENTRIES)
        break;

      status = NtQueryDirectoryObject(dir_handle, buf.data(), static_cast<ULONG>(buf.size()), FALSE, FALSE, &context, NULL);
    }

    CloseHandle(dir_handle);
  }

  return result.c_str();
}
