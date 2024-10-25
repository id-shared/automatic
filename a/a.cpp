#include "DD.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";

int abc = 1;

Ram::Byte raw(Ram::Byte n1, Ram::Byte n2, Ram::Byte n3, Ram::Byte n4) {
  std::cout << abc << ": " << n2 << std::endl;
  abc = abc + 1;

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
  //DD::Contact contact = DD::contact(L"d1.dll");
  //contact.movR(99, 99);
  const int SHM_SIZE = sizeof(Ram::Detail);

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    ptr->n1 = ptr->n1 == 0 ? ptr->n1 : raw(ptr->n1, ptr->n2, ptr->n3, ptr->n4);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
