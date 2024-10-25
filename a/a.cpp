#include "DD.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";

int abc = 1;

int main() {
  //DD::Contact contact = DD::contact(L"d1.dll");
  //contact.movR(99, 99);
  const int SHM_SIZE = sizeof(Ram::Detail);

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    Ram::Byte n4 = ptr->n4;
    Ram::Byte n3 = ptr->n3;
    Ram::Byte n2 = ptr->n2;
    Ram::Byte n1 = ptr->n1;

    if (ptr->n1 != 0) {
      ptr->n1 = 0;

      std::cout << abc << ": " << ptr->n2 << std::endl;
      abc = abc + 1;
    }
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
