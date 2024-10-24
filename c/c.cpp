#include <chrono>
#include <hidapi/hidapi.h>
#include <iostream>
#include <thread>
#include <iomanip>  // For std::setw

int main() {
  // Initialize the hidapi library
  if (hid_init()) {
    std::cerr << "Failed to initialize HID library." << std::endl;
    return 1;
  }

  // Enumerate all HID devices
  struct hid_device_info* devices, * current;
  devices = hid_enumerate(0x046D, 0xC547); // Change the VID and PID if needed
  current = devices;

  // Display all devices and their info
  std::cout << "Available HID devices:" << std::endl;
  int index = 0;
  while (current) {
    std::cout << "Device " << index++ << ":"
      << "\n  Path: " << current->path
      << "\n" << std::endl;
    current = current->next;
  }

  // Get device selection from user
  int selected_device;
  std::cout << "Select a device number to open: ";
  std::cin >> selected_device;

  // Navigate to the selected device
  current = devices;
  for (int i = 0; i < selected_device; ++i) {
    if (current->next == nullptr) {
      std::cerr << "Invalid selection." << std::endl;
      hid_free_enumeration(devices);
      return 1;
    }
    current = current->next;
  }

  // Open the selected HID device
  hid_device* device_handle = hid_open_path(current->path);
  if (!device_handle) {
    std::cerr << "Failed to open device." << std::endl;
    hid_free_enumeration(devices);
    return 1;
  }

  std::cout << "Device successfully opened." << std::endl;

  // Buffer for reading data
  unsigned char buf[8];  // Typical size for mouse input report
  int res;

  // Read data from the device in a loop
  while (true) {
    std::cout << "Attempting to read from device..." << std::endl;
    res = hid_read_timeout(device_handle, buf, sizeof(buf), 100);
    std::cout << "Read result: " << res << std::endl;

    if (res < 0) {
      std::cerr << "Failed to read from device." << std::endl;

      std::this_thread::sleep_for(std::chrono::milliseconds(1000));
      break;
    }
    else if (res == 0) {
      // No data read
      std::this_thread::sleep_for(std::chrono::milliseconds(100));
      continue;
    }

    // Process and display the received data
    std::cout << "Received data: ";
    for (int i = 0; i < res; i++) {
      std::cout << std::setw(2) << std::setfill('0') << std::hex << (int)buf[i] << " ";
    }
    std::cout << std::dec << std::endl;  // Reset to decimal format

    // Parse the mouse input data (first byte often represents button states)
    if (res >= 3) {  // Check if we have enough data for movement
      int buttons = buf[0]; // Button states
      int x = buf[1];       // X movement
      int y = buf[2];       // Y movement

      //std::cout << "Buttons: " << std::bitset<8>(buttons) << " | " << "X: " << x << ", Y: " << y << std::endl;
    }
  }

  // Close the device and clean up
  hid_close(device_handle);
  hid_free_enumeration(devices);
  hid_exit();

  std::cout << "Press Enter to exit..." << std::endl;
  std::cin.get();  // Wait for user input to exit

  return 0;
}
