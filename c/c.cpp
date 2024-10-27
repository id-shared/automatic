#include "Contact.hpp"
#include "Device.hpp"
#include "Driver.hpp"
#include "Xyloid1.hpp"
#include "Xyloid2.hpp"

void main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  Driver::read([driver](std::array<Driver::Byte, 13> o1, std::array<bool, 4> a) {
    Driver::Byte n7 = o1[6];
    Driver::Byte n6 = o1[5];
    Driver::Byte n5 = o1[4];
    Driver::Byte n4 = o1[3];
    Driver::Byte n3 = o1[2];
    Driver::Byte n2 = o1[1];
    Driver::Byte n1 = o1[0];

    //printf("%d, %d, %d, %d, %d, %d, %d.\n", n1, n2, n3, n4, n5, n6, n7);

    int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
    int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;
    int a4 = n1 == 16;
    int a3 = n1 == 8;
    int a2 = n1 == 4;
    int a1 = n1 == 1;

    ax == 0 && ay == 0 ? true : Xyloid2::yx(driver, ay * -1, ax);
    a4 == a[3] ? true : Xyloid2::e2(driver, a4);
    a3 == a[2] ? true : Xyloid2::e1(driver, a3);
    a2 == a[1] ? true : Xyloid2::e2(driver, a2);
    a1 == a[0] ? true : Xyloid1::sf(driver, a1);

    //printf("%d, %d.\n", ay, ax);

    n7 == 0 ? true : Xyloid2::zv(driver, n7 == 255 ? 16 : -16);

    //printf("%d.\n", n7);

    a[3] = a4;
    a[2] = a3;
    a[1] = a2;
    a[0] = a1;
    return a;
    }, 1, 1);
}
