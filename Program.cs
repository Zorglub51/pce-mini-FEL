using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;

class FelPceMini
{
    static UInt16 vid = 0x1F3A;         // Allwinner VID
    static UInt16 pid = 0xEFE8;         // sunxi SoC OTG connector in FEL/flashing mode
    static UInt16 inEndPoint = 0x81;    // Endpoint for USB IN direction
    static UInt16 outEndPoint = 0x02;   // Endpoint for USB OUT direction
    static UsbDevice device = null;
    static UsbEndpointReader epReader = null;
    static UsbEndpointWriter epWriter = null;

    static void Main(string[] args)
    {
        bool found = false;

        // loop until we find the device
        while (!found)
        {
            // Get the list of all connected devices
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;

            // Iterate through all devices and find the one with the correct VID and PID
            foreach (UsbRegistry usbDevice in allDevices)
            {
                if (usbDevice.Vid == vid && usbDevice.Pid == pid)
                {
                    Console.WriteLine("Found the PCE/TG16/Coregrafx Mini!");
                    usbDevice.Open(out device);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("PCE/TG16/Coregrafx Mini not found. Please connect the device and turn it on. Ctrl+C to exit.");
                System.Threading.Thread.Sleep(250);
            }
        }

        // get endpoints
        epReader = device.OpenEndpointReader((ReadEndpointID)inEndPoint, 65536);
        epWriter = device.OpenEndpointWriter((WriteEndpointID)outEndPoint);

        // send the FEL magic
        Console.WriteLine("Sending magic bytes to trigger FEL mode :");
        //byte[] felMagic = [0x55, 0x53, 0x42, 0x43, 0x4A, 0x53, 0x48, 0x4B, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xF8, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        byte[] felMagic = [0x55, 0x50, 0x42, 0x43, 0x4A, 0x53, 0x48, 0x4B, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xF8, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        epWriter.Write(felMagic, 1000, out int bytesWritten);


        Console.Write("FEL activated. Leaving in  3...");
        System.Threading.Thread.Sleep(2000);
        Console.Write("2...");
        System.Threading.Thread.Sleep(2000);
        Console.Write("1...");
        System.Threading.Thread.Sleep(2000);
        if (device != null)
        {
            device.Close();
        }
        if (epReader != null)
        {
            epReader.Dispose();
        }
        if (epWriter != null)
        {
            epWriter.Dispose();
        }
        Console.WriteLine("Bye");
    }
}
