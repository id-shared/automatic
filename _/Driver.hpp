#include <Windows.h>
#include <functional>
#include <string>

namespace Driver {
  constexpr uint32_t STATUS_SUCCESS = 0x00000000;
  constexpr uint32_t STATUS_MORE_ENTRIES = 0x00000105;
  constexpr uint32_t STATUS_BUFFER_TOO_SMALL = 0xC0000023;
  constexpr uint32_t DIRECTORY_QUERY = 0x0001;

  struct UNICODE_STRING {
    USHORT Length;
    USHORT MaximumLength;
    PWSTR Buffer;
  };

  struct OBJECT_ATTRIBUTES {
    ULONG Length;
    HANDLE RootDirectory;
    UNICODE_STRING* ObjectName;
    ULONG Attributes;
    PVOID SecurityDescriptor;
    PVOID SecurityQualityOfService;
  };

  struct OBJECT_DIRECTORY_INFORMATION {
    UNICODE_STRING Name;
    UNICODE_STRING TypeName;
  };

  extern "C" {
    NTSTATUS __stdcall NtOpenDirectoryObject(
      PHANDLE DirectoryHandle,
      ACCESS_MASK DesiredAccess,
      OBJECT_ATTRIBUTES ObjectAttributes
    );

    NTSTATUS __stdcall NtQueryDirectoryObject(
      HANDLE DirectoryHandle,
      PVOID Buffer,
      ULONG Length,
      BOOLEAN ReturnSingleEntry,
      BOOLEAN RestartScan,
      PULONG Context,
      PULONG ReturnLength
    );

    void __stdcall RtlInitUnicodeString(
      UNICODE_STRING* DestinationString,
      PCWSTR SourceString
    );
  }

  inline void RtlInitUnicodeString(UNICODE_STRING* DestinationString, PCWSTR SourceString) {
    if (DestinationString == nullptr) return;

    size_t length = (SourceString != nullptr) ? wcslen(SourceString) * sizeof(wchar_t) : 0;
    DestinationString->Length = static_cast<USHORT>(length);
    DestinationString->MaximumLength = static_cast<USHORT>(length + sizeof(wchar_t));
    DestinationString->Buffer = const_cast<PWSTR>(SourceString);
  }

  std::wstring PtrToStringUni(PWSTR ptr, int length) {
    return std::wstring(ptr, length / sizeof(wchar_t));
  }

  std::wstring FindDevice(std::function<bool(const std::wstring&)> predicate) {
    std::wstring result;
    HANDLE dirHandle = nullptr;

    UNICODE_STRING objName;
    RtlInitUnicodeString(&objName, L"\\GLOBAL??");

    OBJECT_ATTRIBUTES objAttr;
    objAttr.Length = sizeof(OBJECT_ATTRIBUTES);
    objAttr.RootDirectory = nullptr;
    objAttr.ObjectName = &objName;
    objAttr.Attributes = 0;
    objAttr.SecurityDescriptor = nullptr;
    objAttr.SecurityQualityOfService = nullptr;

    if (NtOpenDirectoryObject(&dirHandle, DIRECTORY_QUERY, objAttr) == STATUS_SUCCESS) {
      ULONG context = 0;
      const int bufferSize = 2048;
      auto buffer = reinterpret_cast<OBJECT_DIRECTORY_INFORMATION*>(malloc(bufferSize * sizeof(OBJECT_DIRECTORY_INFORMATION)));
      ULONG returnLength = 0;

      NTSTATUS status = NtQueryDirectoryObject(dirHandle, buffer, bufferSize, FALSE, TRUE, &context, &returnLength);
      while (status == STATUS_SUCCESS || status == STATUS_MORE_ENTRIES) {
        int index = 0;
        while (true) {
          OBJECT_DIRECTORY_INFORMATION& info = buffer[index / sizeof(OBJECT_DIRECTORY_INFORMATION)];
          if (info.Name.Buffer == nullptr) break;

          std::wstring name = PtrToStringUni(info.Name.Buffer, info.Name.Length);
          if (predicate(name)) {
            result = L"\\??\\" + name;
            break;
          }

          index += sizeof(OBJECT_DIRECTORY_INFORMATION);
        }

        if (!result.empty() || status != STATUS_MORE_ENTRIES)
          break;

        status = NtQueryDirectoryObject(dirHandle, buffer, bufferSize, FALSE, FALSE, &context, &returnLength);
      }

      free(buffer);
      CloseHandle(dirHandle);
    }

    return result;
  }
}
