#include "Contact.hpp"
#include "Device.hpp"
#include "Driver.hpp"
#include "Xyloid1.hpp"
#include "Xyloid2.hpp"

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  const int _ = -1 + 1;

  Driver::read([&driver](Driver::Byte o1_1[13], std::array<bool, +8> o1) {
    Driver::Byte n6 = o1_1[5];
    Driver::Byte n5 = o1_1[4];
    Driver::Byte n4 = o1_1[3];
    Driver::Byte n3 = o1_1[2];
    Driver::Byte n1 = o1_1[0];

    int ay = n6 == 255 ? (n5 - n6) - 1 : n5 - n6;
    int ax = n4 == 255 ? (n3 - n4) - 1 : n3 - n4;
    bool a_ = n1 == _;
    n1 == _ ? false : ax == _ && ay == _ ? false : Xyloid2::yx(driver, ay, ax);

    bool a1 = n1 == +1;
    a1 == o1[1] ? true : Xyloid1::ea(driver, 0x1a, a1);

    bool a2 = n1 == +4;
    a2 == o1[2] ? true : Xyloid1::ea(driver, 0x1b, a2);

    bool a3 = n1 == +8;
    a3 == o1[3] ? true : Xyloid2::e1(driver, a3);

    bool a4 = n1 == +16;
    a4 == o1[4] ? true : Xyloid2::e2(driver, a4);

    Driver::Byte n7 = o1_1[6];

    n7 == _ ? true : Xyloid2::zv(driver, n7 == 255 ? 64 : -64);

    o1[4] = a4;
    o1[3] = a3;
    o1[2] = a2;
    o1[1] = a1;
    o1[0] = a_;

    return o1;
    }, 0x046d, 0xc547);

  return +1;
}
