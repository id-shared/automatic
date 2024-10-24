#include <chrono>
#include <hidapi/hidapi.h>
#include <iostream>
#include <thread>

int main() {
  // Initialize the hidapi library
  if (hid_init()) {
    std::cerr << "Failed to initialize HID library." << std::endl;
    return 1;
  }

  // Enumerate all HID devices
  struct hid_device_info* devices, * current;
  devices = hid_enumerate(0x046D, 0xC547); // Enumerate all devices (0x0, 0x0 means all VIDs and PIDs)
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

  // You can now communicate with the device using the `device_handle`
  std::cout << "Listening for mouse packets..." << std::endl;

  unsigned char buf[65]; // Buffer to store data
  while (true) {
    // Read packets from the HID device
    int res = hid_read(device_handle, buf, sizeof(buf));

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

  // Close the device and clean up
  hid_close(device_handle);
  hid_free_enumeration(devices);
  hid_exit();

  std::cout << "Press Enter to exit..." << std::endl;
  std::cin.get();  // Wait for user input to exit

  return 0;
}
