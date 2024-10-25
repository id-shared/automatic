#include "Ram.hpp"
#include "Time.hpp"
#include <iostream>
#include <windows.h>

LPCWSTR SHM_NAME = L"my_shm";
LPCWSTR SEM_NAME = L"my_sem";

int main() {
  HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, SHM_NAME);
  shm_handle != NULL ? shm_handle : throw shm_handle;

  HANDLE sem_handle = OpenSemaphore(SEMAPHORE_MODIFY_STATE, FALSE, SEM_NAME);
  sem_handle != NULL ? sem_handle : throw sem_handle;

  Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(Ram::Detail)));
  ptr != NULL ? ptr : throw ptr;

  while (true) {
    WaitForSingleObject(sem_handle, INFINITE);
    ptr->n4 = 1;
    ptr->n3 = 1;
    ptr->n2 = 1;
    ptr->n1 = 1;
    ReleaseSemaphore(sem_handle, 1, NULL);
  }

  return 0;
}
