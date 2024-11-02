#pragma once

#include <iostream>
#include <Windows.h>

namespace Ram {
  using Byte = unsigned char;

  struct Detail {
    Byte n4;
    Byte n3;
    Byte n2;
    Byte n1;
  };

  static bool XO(double ms) {
    HANDLE shm_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, L"ram");
    shm_handle != NULL ? shm_handle : throw shm_handle;

    HANDLE sem_handle = OpenSemaphore(SEMAPHORE_ALL_ACCESS, FALSE, L"ram");
    sem_handle != NULL ? sem_handle : throw sem_handle;

    Ram::Detail* ptr = static_cast<Ram::Detail*>(MapViewOfFile(shm_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(Ram::Detail)));
    ptr != NULL ? ptr : throw ptr;

    WaitForSingleObject(sem_handle, INFINITE);
    ptr->n4 = 1;
    ptr->n3 = 1;
    ptr->n2 = 1;
    ptr->n1 = 1;
    ReleaseSemaphore(sem_handle, 1, NULL);

    WaitForSingleObject(sem_handle, INFINITE);
    ptr->n4 = 1;
    ptr->n3 = 1;
    ptr->n2 = 1;
    ptr->n1 = 2;
    ReleaseSemaphore(sem_handle, 1, NULL);

    return true;
  }

  static bool end(HANDLE shm_handle, Detail* ptr) {
    UnmapViewOfFile(ptr);
    CloseHandle(shm_handle);
    return true;
  }
}
