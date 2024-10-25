#include "Ram.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";

int main() {
  HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(Ram::Detail)));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    ptr->n4 = 1;
    ptr->n3 = 1;
    ptr->n2 = 1;
    ptr->n1 = 1;

    std::cout << ptr->n1 << ": " << ptr->n2 << std::endl;
  }

  return 0;
}
