#include "DD.hpp"
#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";

int abc = 1;

int main() {
  DD::Contact contact = DD::contact(L"d1.dll");
  //contact.movR(99, 99);
  const int SHM_SIZE = sizeof(Ram::Data);

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  Ram::Data* ptr = static_cast<Ram::Data*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    if (ptr->flag == 1) {
      abc = abc + 1;
      std::cout << abc << ": " << ptr->data << std::endl;
      ptr->flag = 0;
    }
    Time::XO(0.01);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
