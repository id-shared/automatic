#include <iostream>
#include <windows.h>
#include <cstring>
#include <thread>
#include <cstdlib>

LPCWSTR SHM_NAME = L"my_shm";  // Windows uses a different naming convention
LPCWSTR SEM_NAME = L"my_sem";  // Same here
const int SHM_SIZE = 4;            // 4 bytes

int main() {
  // Open existing shared memory
  HANDLE shm_handle = OpenFileMapping(
    FILE_MAP_ALL_ACCESS,      // Read/write access
    FALSE,                    // Do not inherit the name
    SHM_NAME                  // Name of the mapping object
  );

  if (shm_handle == NULL) {
    std::cerr << "Could not open file mapping object: " << GetLastError() << std::endl;
    return 1;
  }

  void* ptr = MapViewOfFile(
    shm_handle,              // Handle to map object
    FILE_MAP_ALL_ACCESS,     // Read/write access
    0,                       // Offset (high-order DWORD)
    0,                       // Offset (low-order DWORD)
    SHM_SIZE                 // Size of mapping
  );

  if (ptr == NULL) {
    std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
    CloseHandle(shm_handle);
    return 1;
  }

  // Open existing semaphore
  HANDLE sem = OpenSemaphore(
    SEMAPHORE_MODIFY_STATE,   // Required access
    FALSE,                    // Do not inherit the name
    SEM_NAME                  // Semaphore name
  );

  if (sem == NULL) {
    std::cerr << "Could not open semaphore: " << GetLastError() << std::endl;
    UnmapViewOfFile(ptr);
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    // Generate 4 bytes of data (for example, random bytes)
    uint32_t data = rand() % 0xFFFFFFFF;

    // Wait for semaphore (equivalent to sem_wait)
    WaitForSingleObject(sem, INFINITE);

    // Write data to shared memory
    memcpy(ptr, &data, SHM_SIZE);

    // Signal semaphore (equivalent to sem_post)
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
