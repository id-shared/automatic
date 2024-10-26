#include "Driver.hpp"
#include <iostream>
#include <libusb-1.0/libusb.h>
#include <ntddkbd.h>
#include <ntddmou.h>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";

struct D1Control {
  uint32_t unk1;
  enum class Type : uint32_t {
    Keyboard = 1,
    Mouse = 2
  } type;
  union {
    KEYBOARD_INPUT_DATA ki;
    MOUSE_INPUT_DATA mi;
  };
private:
  void assert_size() {
    static_assert(sizeof D1Control == 32);
  }
};

bool ab(HANDLE x1, D1Control x) {
  DWORD bytes_returned;
  return DeviceIoControl(x1, 0x88883020, &x, sizeof x, nullptr, 0, &bytes_returned, nullptr);
}

bool yx(HANDLE x1, int y, int x) {
  return ab(x1, D1Control{
    .type = D1Control::Type::Mouse,
    .mi = MOUSE_INPUT_DATA {
      .LastX = x,
      .LastY = y,
    },
    });
}

bool ee(HANDLE x1, bool e) {
  ULONG buttons = e ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP;

  return ab(x1, D1Control{
    .type = D1Control::Type::Mouse,
    .mi = MOUSE_INPUT_DATA {
      .Buttons = buttons
    },
    });
}

void main() {
  LPCWSTR device_name = find_device([](std::wstring_view sv) {
    using namespace std::literals;
    return sv.starts_with(L"RZCONTROL#"sv) && sv.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
  });

  std::wcout << device_name << std::endl;

  HANDLE device = CreateFileW(device_name, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
  device != INVALID_HANDLE_VALUE ? device : throw device;

  int configuration = 1;
  int interface = 0;

  libusb_context* ctx = NULL;
  libusb_device** devs;
  ssize_t cnt;
  libusb_device_handle* handle = NULL;

  libusb_init(&ctx);

  cnt = libusb_get_device_list(ctx, &devs);
  for (ssize_t i = 0; i < cnt; i++) {
    struct libusb_device_descriptor desc;
    libusb_get_device_descriptor(devs[i], &desc);
    if (desc.idVendor == 0x046d && desc.idProduct == 0xc547) {
      libusb_open(devs[i], &handle);
      break;
    }
  }

  if (handle == NULL) {
    libusb_free_device_list(devs, 1);
    libusb_exit(ctx);
    throw cnt;
  }

  libusb_set_configuration(handle, configuration);
  libusb_claim_interface(handle, interface);

  using Byte = unsigned char;
  Byte data[13];
  int actual_length;
  bool x1 = false;

  while (true) {
    int res = libusb_interrupt_transfer(handle, 0x81, data, sizeof(data), &actual_length, 0);
    if (res == 0) {
      Byte n13 = data[12];
      Byte n12 = data[11];
      Byte n11 = data[10];
      Byte n10 = data[9];
      Byte n9 = data[8];
      Byte n8 = data[7];
      Byte n7 = data[6];
      Byte n6 = data[5];
      Byte n5 = data[4];
      Byte n4 = data[3];
      Byte n3 = data[2];
      Byte n2 = data[1];
      Byte n1 = data[0];

      //printf("%d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d.\n", n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13);

      int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
      int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;

      if (ax == 0 && ay == 0) {

      }
      else {
        printf("%d, %d\n", ax, ay);
        yx(device, ay * -1, ax);
      }

      int x1_ = n1 == 1;
      if (x1_ == x1) {

      }
      else {
        printf("%d\n", x1_);
        ee(device, x1_);
      }
      x1 = x1_;
    }
    else {
      printf("Error reading data: %d (%s).\n", res, libusb_error_name(res));
    }
  }

  libusb_release_interface(handle, interface);
  libusb_close(handle);
  libusb_free_device_list(devs, 1);
  libusb_exit(ctx);
}
