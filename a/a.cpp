#include <cstring>
#include <iostream>
#include <windows.h>
#include <thread>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";

struct SharedData {
  uint32_t data;
  uint32_t flag;
};

int abc = 1;

int main() {
  const int SHM_SIZE = sizeof(SharedData);

  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  if (shm_handle == NULL) {
    std::cerr << "Could not create shared memory: " << GetLastError() << std::endl;
    return 1;
  }

  HANDLE sem_handle = CreateSemaphore(NULL, 1, 1, SEM_NAME);
  if (sem_handle == NULL) {
    std::cerr << "Could not create semaphore: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  SharedData* ptr = static_cast<SharedData*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE));
  if (ptr == NULL) {
    std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
    CloseHandle(sem_handle);
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    WaitForSingleObject(sem_handle, INFINITE);

    if (ptr->flag == 1) {
      abc = abc + 1;
      std::cout << "Received data: " << abc << " " << ptr->data << std::endl;
      ptr->flag = 0;
    }

    ReleaseSemaphore(sem_handle, 1, NULL);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem_handle);
  return 0;
}
