#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

SIZE_T _SHM_SIZE = sizeof(Ram::Detail);
LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";
int CURRENT_NAME = 1;

Ram::Byte raw(Ram::Byte n1, Ram::Byte n2, Ram::Byte n3, Ram::Byte n4) {
  switch (n1) {
  case 2:
    printf("2: %d %d.\n", n2, n3);
    return 0;
  case 1:
    printf("1: %d.\n", n2);
    return 0;
  default:
    return 0;
  }
}

void main() {
  HANDLE shm_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, _SHM_SIZE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  HANDLE sem_handle = CreateSemaphore(NULL, 1, 1, SEM_NAME);
  sem_handle != NULL ? sem_handle : throw sem_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, _SHM_SIZE));
  ptr != NULL ? ptr : throw ptr;

  ptr->n1 = 0;
  while (true) {
    WaitForSingleObject(sem_handle, INFINITE);
    //std::cout << CURRENT_NAME << "\n" << std::endl;
    //CURRENT_NAME = CURRENT_NAME + 1;
    ptr->n1 = ptr->n1 == 0 ? ptr->n1 : raw(ptr->n1, ptr->n2, ptr->n3, ptr->n4);
    ReleaseSemaphore(sem_handle, 1, NULL);
  }
}
