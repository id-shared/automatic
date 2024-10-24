#include <libusb-1.0/libusb.h>
#include <stdio.h>
#include <signal.h>
#include <stdbool.h>

volatile bool keep_running = true;

void handle_sigint(int sig) {
  keep_running = false;
}

int main() {
  libusb_context* ctx = NULL;
  libusb_device** devs;
  ssize_t cnt;
  libusb_device_handle* handle = NULL;

  // Register signal handler
  signal(SIGINT, handle_sigint);

  // Initialize libusb
  libusb_init(&ctx);

  // Find the device (using VID and PID of your mouse)
  cnt = libusb_get_device_list(ctx, &devs);
  for (ssize_t i = 0; i < cnt; i++) {
    struct libusb_device_descriptor desc;
    libusb_get_device_descriptor(devs[i], &desc);

    printf("Device found: VID: %04x, PID: %04x\n", desc.idVendor, desc.idProduct);

    if (desc.idVendor == YOUR_VENDOR_ID && desc.idProduct == YOUR_PRODUCT_ID) {
      libusb_open(devs[i], &handle);
      break;
    }
  }

  if (handle == NULL) {
    printf("Device not found.\n");
    libusb_free_device_list(devs, 1);
    libusb_exit(ctx);
    return 1; // Exit if device not found
  }

  // Claim the interface and set the configuration
  libusb_set_configuration(handle, 1); // Adjust as necessary
  libusb_claim_interface(handle, 0);    // Adjust as necessary

  // Set up the endpoint and read data
  unsigned char data[8]; // Adjust size based on the report length
  int actual_length;

  while (keep_running) {
    int res = libusb_interrupt_transfer(handle, YOUR_ENDPOINT, data, sizeof(data), &actual_length, 0);
    if (res == 0) {
      // Process the data
      int x_movement = data[1]; // Typically X movement is at index 1
      int y_movement = data[2]; // Y movement at index 2
      printf("X: %d, Y: %d\n", x_movement, y_movement);
    }
    else {
      printf("Error reading data: %d (%s)\n", res, libusb_error_name(res));
    }
  }

  // Cleanup
  libusb_release_interface(handle, 0); // Release interface
  libusb_close(handle);
  libusb_free_device_list(devs, 1);
  libusb_exit(ctx);

  return 0;
}
