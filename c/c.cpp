#include "Contact.hpp"
#include "Device.hpp"
#include "Driver.hpp"
#include "Mouse.hpp"

void main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  Driver::read([driver](std::array<Driver::Byte, 13> o1, std::array<bool, 2> a) {
    Driver::Byte n13 = o1[12];
    Driver::Byte n12 = o1[11];
    Driver::Byte n11 = o1[10];
    Driver::Byte n10 = o1[9];
    Driver::Byte n9 = o1[8];
    Driver::Byte n8 = o1[7];
    Driver::Byte n7 = o1[6];
    Driver::Byte n6 = o1[5];
    Driver::Byte n5 = o1[4];
    Driver::Byte n4 = o1[3];
    Driver::Byte n3 = o1[2];
    Driver::Byte n2 = o1[1];
    Driver::Byte n1 = o1[0];

    printf("%d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d.\n", n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13);

    int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
    int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;
    int a2 = n1 == 4;
    int a1 = n1 == 1;

    ax == 0 && ay == 0 ? true : Mouse::yx(driver, ay * -1, ax);
    a2 == a[1] ? true : Mouse::e2(driver, a2);
    a1 == a[0] ? true : Mouse::e1(driver, a1);

    printf("%d, %d, %d, %d\n", ay, ax, a2, a1);

    Mouse::zv(driver, 10);

    a[1] = a2;
    a[0] = a1;
    return a;
    }, 1, 1);
}
