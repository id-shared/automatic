#include <iostream>
#include <windows.h>
#include "Time.hpp"

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
  if (shm_handle == NULL) {
    std::cerr << "Could not open file mapping object: " << GetLastError() << std::endl;
    return 1;
  }

  SharedData* ptr = static_cast<SharedData*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(SharedData)));
  if (ptr == NULL) {
    std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    ptr->flag = 1;
    ptr->data = rand() % 100;
    abc = abc + 1;
    std::cout << "Sent data: " << abc << " " << ptr->data << std::endl;
    Time::XO(1);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
