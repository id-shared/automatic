#include <cstring>
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";
const int SHM_SIZE = 4; // 4 bytes
const uint32_t SENTINEL_VALUE = 0xFFFFFFFF; // Example sentinel value indicating client disconnection

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

  void* ptr = MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE);
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

    // Check for the sentinel value indicating client disconnection
    if (data == SENTINEL_VALUE) {
      std::cout << "Client has disconnected." << std::endl;
      break; // Exit the loop if the client has disconnected
    }

    std::cout << "Received data: " << data << std::endl;

    // Signal semaphore
    ReleaseSemaphore(sem_handle, 1, NULL);

    // Optional: Sleep to reduce CPU usage
    Sleep(10); // Sleep for 10 milliseconds or adjust as needed
  }

  // Cleanup
  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem_handle);
  return 0;
}
