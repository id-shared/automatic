#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";

struct SharedData {
  uint32_t data;
  uint32_t flag;
};

int abc = 1;

bool XO(double ms) {
  LARGE_INTEGER frequency;
  LARGE_INTEGER start;

  QueryPerformanceFrequency(&frequency);
  QueryPerformanceCounter(&start);

  double ticksToWait = (ms / 1000.0) * frequency.QuadPart;
  LARGE_INTEGER current;

  do {
    QueryPerformanceCounter(&current);
  } while (current.QuadPart - start.QuadPart < ticksToWait);

  return true;
}

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
    ptr->data = rand() % 100;
    ptr->flag = 1;
    abc = abc + 1;
    std::cout << "Sent data: " << abc << " " << ptr->data << std::endl;
    XO(1);
  }

  UnmapViewOfFile(ptr);
  CloseHandle(shm_handle);
  return 0;
}
