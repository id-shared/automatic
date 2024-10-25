#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";

struct SharedData {
  uint32_t data;
  uint32_t flag;
};

int abc = 1;

int main() {
  const int SHM_SIZE = sizeof(SharedData);

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  SharedData* ptr = static_cast<SharedData*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
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
