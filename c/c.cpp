#include <libusb-1.0/libusb.h>
#include <stdio.h>
#include <signal.h>
#include <stdbool.h>

volatile bool keep_running = true;

int main() {
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
      int n7 = data[7];
      int n6 = data[6];
      int n5 = data[5];
      int n4 = data[4];
      int n3 = data[3];
      int n2 = data[2];
      int n1 = data[1];
      int n = data[0];
      printf("%d, %d, %d, %d, %d, %d, %d, %d\n", n7, n6, n5, n4, n3, n2, n1, n);
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
