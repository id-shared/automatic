#include <iostream>
#include <windows.h>
#include <cstring>
#include <thread>
#include <cstdlib>
#include <ctime>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";
const int SHM_SIZE = sizeof(uint32_t) * 2;

struct SharedData {
  uint32_t data;
  uint32_t flag;
};

int main() {
  srand(static_cast<unsigned int>(time(NULL)));

  HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, SHM_NAME);
  if (shm_handle == NULL) {
    std::cerr << "Could not open file mapping object: " << GetLastError() << std::endl;
    return 1;
  }

  SharedData* ptr = static_cast<SharedData*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
  if (ptr == NULL) {
    std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  HANDLE sem = OpenSemaphore(SEMAPHORE_MODIFY_STATE, FALSE, SEM_NAME);
  if (sem == NULL) {
    std::cerr << "Could not open semaphore: " << GetLastError() << std::endl;
    UnmapViewOfFile(ptr);
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    uint32_t data = rand() % 0xFFFFFFFF;
    WaitForSingleObject(sem, INFINITE);
    ptr->data = data;
    ptr->flag = 1;
    ReleaseSemaphore(sem, 1, NULL);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem);
  return 0;
}
