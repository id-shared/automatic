#include <iostream>
#include <hidapi/hidapi.h>
#include <thread>
#include <chrono>

void logMousePackets() {
  // Initialize HIDAPI
  if (hid_init() != 0) {
    std::cerr << "Failed to initialize HIDAPI." << std::endl;
    return;
  }

  // Open the HID device (replace with your mouse's VID and PID)
  const uint16_t VID = 0x046D; // Example: Logitech
  const uint16_t PID = 0xC53F; // Example: G602
  hid_device* handle = hid_open(VID, PID, nullptr);

  if (!handle) {
    std::cerr << "Unable to open HID device." << std::endl;
    hid_exit();
    return;
  }

  std::cout << "Listening for mouse packets..." << std::endl;

  unsigned char buf[65]; // Buffer to store data
  while (true) {
    // Read packets from the HID device
    int res = hid_read(handle, buf, sizeof(buf));

    if (res > 0) {
      std::cout << "Received: ";
      for (int i = 0; i < res; ++i) {
        std::cout << std::hex << (int)buf[i] << " ";
      }
      std::cout << std::dec << std::endl; // Switch back to decimal
    }
    else if (res < 0) {
      std::cerr << "Error reading from device." << std::endl;
      break; // Exit on error
    }

    std::this_thread::sleep_for(std::chrono::milliseconds(100)); // Throttle reading
  }

  // Close the HID device and cleanup
  hid_close(handle);
  hid_exit();
}

int main() {
  logMousePackets();
  return 0;
}
