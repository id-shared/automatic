#include <libusb-1.0/libusb.h>
#include <signal.h>
#include <stdbool.h>
#include <stdio.h>
#include "Dll.hpp"

volatile bool keep_running = true;

int main() {
  HMODULE contact = Dll::dll(L"d1.dll");

  typedef int (WINAPI* DD_movR)(int, int);
  DD_movR movR = Dll::fn<DD_movR>(contact, "DD_movR");

  typedef int (WINAPI* DD_btn)(int);
  DD_btn btn = Dll::fn<DD_btn>(contact, "DD_btn");

  printf("result: %d\n", btn(0));

  libusb_context* ctx = NULL;
  libusb_device** devs;
  ssize_t cnt;
  libusb_device_handle* handle = NULL;

  // Initialize libusb
  libusb_init(&ctx);

  // Find the device (using VID and PID of your mouse)
  cnt = libusb_get_device_list(ctx, &devs);
  for (ssize_t i = 0; i < cnt; i++) {
    struct libusb_device_descriptor desc;
    libusb_get_device_descriptor(devs[i], &desc);

    printf("Device found: VID: %04x, PID: %04x\n", desc.idVendor, desc.idProduct);

    if (desc.idVendor == 0x046d && desc.idProduct == 0xc547) {
      libusb_open(devs[i], &handle);
      break;
    }
  }

  if (handle == NULL) {
    printf("Device not found.\n");
    libusb_free_device_list(devs, 1);
    libusb_exit(ctx);
    return 1;
  }

  int config = 1;
  int interface = 0;
  libusb_set_configuration(handle, config);
  libusb_claim_interface(handle, interface);

  unsigned char data[8];
  int actual_length;

  while (keep_running) {
    int res = libusb_interrupt_transfer(handle, 0x81, data, sizeof(data), &actual_length, 0);
    if (res == 0) {
      int n8 = data[7];
      int n7 = data[6];
      int n6 = data[5];
      int n5 = data[4];
      int n4 = data[3];
      int n3 = data[2];
      int n2 = data[1];
      int n1 = data[0];
      /*if (n6 == 0 || n6 == 255) {

      }
      else {
        printf("%d\n", n6);
      }
      if (n4 == 0 || n4 == 255) {

      }
      else {
        printf("%d\n", n4);
      }*/
      if (n1 == 1 && n2 == 147) {
      }
      else {
        int ax = (n4 == 255 ? (n3 - n4) - 1 : n3 - n4) * +1;
        int ay = (n6 == 255 ? (n5 - n6) - 1 : n5 - n6) * -1;
        //printf("%d, %d\n", ax, ay);
        movR(ax, ay * -1);
      }
      //printf("%d, %d, %d, %d, %d\n", n3, n4, n5, n6, n7);
      //printf("%d, %d\n", n1, n2);
    }
    else {
      printf("Error reading data: %d (%s)\n", res, libusb_error_name(res));
    }
  }

  libusb_release_interface(handle, interface);
  libusb_close(handle);
  libusb_free_device_list(devs, 1);
  libusb_exit(ctx);

  return 0;
}
