#pragma once
#include <array>
#include <libusb-1.0/libusb.h>

namespace Driver {
  using Byte = unsigned char;

  bool read(std::function<std::array<bool, 4>(Byte[13], std::array<bool, 4>)> z, uint16_t c_1, uint16_t c) {
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
      if (desc.idVendor == c_1 && desc.idProduct == c) {
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

    std::array<bool, 4> back = {};
    Byte data[13];
    int size;

    while (true) {
      int re = libusb_interrupt_transfer(handle, 0x81, data, sizeof(data), &size, 0);
      re == 0 && size > 0 ? back = z(data, back) : back;
    }

    libusb_release_interface(handle, interface);
    libusb_close(handle);
    libusb_free_device_list(devs, 1);
    libusb_exit(ctx);

    return true;
  }
}
