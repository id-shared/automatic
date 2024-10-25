#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";

struct SharedData {
  uint32_t data;
  uint32_t flag;
};

int abc = 1;

int main() {
  srand(static_cast<unsigned int>(time(NULL)));

  HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  SharedData* ptr = static_cast<SharedData*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(SharedData)));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    ptr->flag = 1;
    ptr->data = rand() % 100;
    abc = abc + 1;
    std::cout << abc << ": " << ptr->data << std::endl;
    Time::XO(0.01);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
