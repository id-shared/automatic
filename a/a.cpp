#include "DD.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

SIZE_T _SHM_SIZE = sizeof(Ram::Detail);
LPCWSTR SHM_NAME = L"my_shm";

//DD::Contact contact = DD::contact(L"d1.dll");

Ram::Byte raw(Ram::Byte n1, Ram::Byte n2, Ram::Byte n3, Ram::Byte n4) {
  //contact.movR(99, 99);

  std::cout << n1 << ": " << n2 << std::endl;

  switch (n1) {
  case 3:
    std::cout << "Value is 3\n";
    return 0;
  case 2:
    std::cout << "Value is 2\n";
    return 0;
  case 1:
    std::cout << "Value is 1\n";
    return 0;
  default:
    return 0;
  }
}

int main() {

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, _SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, _SHM_SIZE));
  ptr != NULL ? ptr : throw ptr;

  ptr->n1 = 0;
  while (true) {
    //std::cout << ptr->n1 << ": " << ptr->n2 << std::endl;

    ptr->n1 = ptr->n1 == 0 ? ptr->n1 : raw(ptr->n1, ptr->n2, ptr->n3, ptr->n4);
  }

  return 0;
}
