#include "Contact.hpp"
#include "Device.hpp"
#include "Driver.hpp"
#include "Mouse.hpp"

void main() {
  LPCWSTR driver = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL#"sv) && c.ends_with(L"#{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE device = Driver::device(driver);

  Device::read([device](std::array<Device::Byte, 13> o1, std::array<bool, 2> a) {
    Device::Byte n13 = o1[12];
    Device::Byte n12 = o1[11];
    Device::Byte n11 = o1[10];
    Device::Byte n10 = o1[9];
    Device::Byte n9 = o1[8];
    Device::Byte n8 = o1[7];
    Device::Byte n7 = o1[6];
    Device::Byte n6 = o1[5];
    Device::Byte n5 = o1[4];
    Device::Byte n4 = o1[3];
    Device::Byte n3 = o1[2];
    Device::Byte n2 = o1[1];
    Device::Byte n1 = o1[0];

    printf("%d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d, %d.\n", n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13);

    int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
    int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;
    int a1 = n1 == 1;

    //printf("%d, %d\n", ax, ay);

    ax == 0 && ay == 0 ? true : Mouse::yx(device, ay * -1, ax);
    a1 == a[0] ? true : Mouse::ee(device, a1);

    a[0] = a1;
    return a;
    }, 1, 1);
}
