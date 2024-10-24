#include <iostream>
#include <hidapi/hidapi.h>
#include <string>
#include <vector>

struct DeviceInfo {
    std::wstring path;
    std::wstring serial_number;
    std::wstring manufacturer;
    std::wstring product;
    uint16_t vendor_id;
    uint16_t product_id;
};

void listAndSelectDevice() {
    std::vector<DeviceInfo> devices;

    // Enumerate all HID devices
    struct hid_device_info* devs, * cur_dev;
    devs = hid_enumerate(0x046D, 0xC53F);  // Example VID and PID (replace with your own)
    cur_dev = devs;

    int index = 0;

    while (cur_dev) {
        DeviceInfo info;
        info.path = cur_dev->path;
        info.serial_number = cur_dev->serial_number ? cur_dev->serial_number : L"N/A";
        info.manufacturer = cur_dev->manufacturer_string ? cur_dev->manufacturer_string : L"N/A";
        info.product = cur_dev->product_string ? cur_dev->product_string : L"N/A";
        info.vendor_id = cur_dev->vendor_id;
        info.product_id = cur_dev->product_id;

        devices.push_back(info);

        // Display device information
        std::wcout << L"Device #" << index << L":" << std::endl;
        std::wcout << L"  Path: " << info.path << std::endl;
        std::wcout << L"  Vendor ID: " << std::hex << info.vendor_id << std::endl;
        std::wcout << L"  Product ID: " << std::hex << info.product_id << std::endl;
        std::wcout << L"  Serial Number: " << info.serial_number << std::endl;
        std::wcout << L"  Manufacturer: " << info.manufacturer << std::endl;
        std::wcout << L"  Product: " << info.product << std::endl;
        std::wcout << std::endl;

        cur_dev = cur_dev->next;
        index++;
    }

    if (devices.empty()) {
        std::cerr << "No devices found." << std::endl;
        hid_free_enumeration(devs);
        return;
    }

    // Prompt the user to select a device
    int choice = 0;
    std::cout << "Select a device (0 - " << devices.size() - 1 << "): ";
    std::cin >> choice;

    if (choice < 0 || choice >= devices.size()) {
        std::cerr << "Invalid selection." << std::endl;
        hid_free_enumeration(devs);
        return;
    }

    // Open the selected device
    DeviceInfo selectedDevice = devices[choice];
    hid_device* handle = hid_open_path(selectedDevice.path.c_str());

    if (!handle) {
        std::cerr << "Unable to open the selected HID device." << std::endl;
        hid_free_enumeration(devs);
        return;
    }

    std::cout << "Listening for packets from selected device..." << std::endl;

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
        } else if (res < 0) {
            std::cerr << "Error reading from device." << std::endl;
            break; // Exit on error
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(100)); // Throttle reading
    }

    // Clean up
    hid_close(handle);
    hid_free_enumeration(devs);
}

int main() {
    // Initialize HIDAPI
    if (hid_init() != 0) {
        std::cerr << "Failed to initialize HIDAPI." << std::endl;
        return 1;
    }

    // List and select a device
    listAndSelectDevice();

    // Cleanup HIDAPI
    hid_exit();
    return 0;
}
