#include <cstring>
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";
const int SHM_SIZE = 4; // 4 bytes

int main() {
  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHM_SIZE, SHM_NAME);
  if (shm_handle == NULL) {
    std::cerr << "Could not create shared memory: " << GetLastError() << std::endl;
    return 1;
  }

  HANDLE sem_handle = CreateSemaphore(NULL, 0, 1, SEM_NAME);
  if (sem_handle == NULL) {
    std::cerr << "Could not create semaphore: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  void* ptr = MapViewOfFile(shm_handle, FILE_MAP_READ, 0, 0, SHM_SIZE);
  if (ptr == NULL) {
    std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    // Wait for semaphore
    WaitForSingleObject(sem_handle, INFINITE);

    // Read data from shared memory
    uint32_t data;
    memcpy(&data, ptr, SHM_SIZE);
    std::cout << "Received data: " << data << std::endl;

    // Signal semaphore
    ReleaseSemaphore(sem_handle, 1, NULL);

    // You can sleep or process data further as needed
  }

  // Cleanup (not reached in this example)
  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem_handle);
  return 0;
}
