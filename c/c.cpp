#include <iostream>
#include <windows.h>
#include <cstring>
#include <thread>
#include <cstdlib>
#include <ctime>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";
const int SHM_SIZE = 4; // 4 bytes

int main() {
  srand(static_cast<unsigned int>(time(NULL))); // Seed random number generator

  HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, SHM_NAME);
  if (shm_handle == NULL) {
    std::cerr << "Could not open file mapping object: " << GetLastError() << std::endl;
    return 1;
  }

  void* ptr = MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, SHM_SIZE);
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
    // Generate 4 bytes of data (for example, random bytes)
    uint32_t data = rand() % 0xFFFFFFFF;

    // Wait for semaphore
    WaitForSingleObject(sem, INFINITE);

    // Write data to shared memory
    memcpy(ptr, &data, SHM_SIZE);

    // Signal semaphore
    ReleaseSemaphore(sem, 1, NULL);

    // Sleep for 1 microsecond
    std::this_thread::sleep_for(std::chrono::microseconds(1));
  }

  // Cleanup (not reached in this example)
  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem);
  return 0;
}
