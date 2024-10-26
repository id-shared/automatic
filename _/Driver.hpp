#pragma once
#include <array>
#include <libusb-1.0/libusb.h>

namespace Driver {
  using Byte = unsigned char;

  LPCWSTR read(std::function<std::array<bool, 2>(std::array<Byte, 13>, std::array<bool, 2>)> z, uint16_t c_1, uint16_t c) {
    int configuration = 1;
    int interface = 0;

    libusb_context* ctx = NULL;
    libusb_device** devs;
    ssize_t cnt;
    libusb_device_handle* handle = NULL;

    libusb_init(&ctx);

    cnt = libusb_get_device_list(ctx, &devs);
    for (ssize_t i = 0; i < cnt; i++) {
      struct libusb_device_descriptor desc;
      libusb_get_device_descriptor(devs[i], &desc);
      if (desc.idVendor == 0x046d && desc.idProduct == 0xc547) {
        libusb_open(devs[i], &handle);
        break;
      }
    }

    if (handle == NULL) {
      libusb_free_device_list(devs, 1);
      libusb_exit(ctx);
      throw cnt;
    }

    libusb_set_configuration(handle, configuration);
    libusb_claim_interface(handle, interface);

    std::array<bool, 2> button = {};
    std::array<Byte, 13> data = {};
    int actual_length;

    while (true) {
      int response = libusb_interrupt_transfer(handle, 0x81, data.data(), sizeof(data), &actual_length, 0);
      button = z(data, button);
    }

    libusb_release_interface(handle, interface);
    libusb_close(handle);
    libusb_free_device_list(devs, 1);
    libusb_exit(ctx);
    return L"";
  }
}
