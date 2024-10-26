#include "Hardware.hpp"
#include "Driver.hpp"
#include "Usb.hpp"
#include <iostream>
#include <libusb-1.0/libusb.h>
#include <ntddkbd.h>
#include <ntddmou.h>
#include <windows.h>

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

template<typename T>
void show(const T& x) {
  std::wcout << x << std::endl;
}

void main() {
  LPCWSTR device_name = Hardware::find_device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
  });

  HANDLE device = Driver::device(device_name);

  Usb::read([device](std::array<Usb::Byte, 13> o1, std::array<bool, 2> a) {
    //printf("%d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d.\n", n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13);

    Usb::Byte n13 = o1[12];
    Usb::Byte n12 = o1[11];
    Usb::Byte n11 = o1[10];
    Usb::Byte n10 = o1[9];
    Usb::Byte n9 = o1[8];
    Usb::Byte n8 = o1[7];
    Usb::Byte n7 = o1[6];
    Usb::Byte n6 = o1[5];
    Usb::Byte n5 = o1[4];
    Usb::Byte n4 = o1[3];
    Usb::Byte n3 = o1[2];
    Usb::Byte n2 = o1[1];
    Usb::Byte n1 = o1[0];

    int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
    int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;

    if (ax == 0 && ay == 0) {

    }
    else {
      printf("%d, %d\n", ax, ay);
      yx(device, ay * -1, ax);
    }

    int x1_ = n1 == 1;
    if (x1_ == a[0]) {

    }
    else {
      printf("%d\n", x1_);
      ee(device, x1_);
    }
    a[0] = x1_;
    return a;
  }, 1, 1);
}
