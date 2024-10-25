#include <iostream>
#include <windows.h>
#include <cstring>
#include <thread>
#include <cstdlib>
#include <ctime>

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

  HANDLE sem = OpenSemaphore(SEMAPHORE_MODIFY_STATE, FALSE, SEM_NAME);
  if (sem == NULL) {
    std::cerr << "Could not open semaphore: " << GetLastError() << std::endl;
    UnmapViewOfFile(ptr);
    CloseHandle(shm_handle);
    return 1;
  }

  while (true) {
    uint32_t data = 100;
    WaitForSingleObject(sem, INFINITE);
    ptr->data = data;
    ptr->flag = 1;
    abc = abc + 1;
    std::cout << "Sent data: " << abc << " " << ptr->data << std::endl;
    ReleaseSemaphore(sem, 1, NULL);
    std::this_thread::sleep_for(std::chrono::microseconds(1));
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  CloseHandle(sem);
  return 0;
}
