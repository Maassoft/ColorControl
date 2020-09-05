using NLog;
using System;
using System.Globalization;
using PacketDotNet;
using System.Net.NetworkInformation;
using SharpPcap;
using System.Diagnostics;
using SharpPcap.Npcap;
using System.Linq;

namespace ColorControl
{
    class WOL
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //You need SharpPcap for this to work

        public static void WakeFunction(string macAddress)
        {
            /* Retrieve the device list */
            var devices = CaptureDeviceList.Instance;

            Logger.Debug("Waking device with MAC-address " + macAddress);

            /*If no device exists, print error */
            if (devices.Count < 1)
            {
                Logger.Debug("No network device found on this machine");
                return;
            }

            foreach (var device in devices)
            {
                //if (device is NpcapDevice npcapDevice)
                //{
                //    if (!npcapDevice.Interface.GatewayAddresses.Any())
                //    {
                //        continue;
                //    }
                //}

                Logger.Debug("Opening network device: " + device.Name);

                //Open the device
                device.Open();

                //A magic packet is a broadcast frame containing anywhere within its payload: 6 bytes of ones
                //(resulting in hexadecimal FF FF FF FF FF FF), followed by sixteen repetitions 

                string response = macAddress.Replace(":", "-");

                var address = PhysicalAddress.Parse("FFFFFFFFFFFF");
                EthernetPacket ethernet = new EthernetPacket(address, address, EthernetType.WakeOnLan);
                ethernet.PayloadPacket = new WakeOnLanPacket(PhysicalAddress.Parse(response));

                byte[] bytes = ethernet.Bytes;

                try
                {
                    //Send the packet out the network device
                    device.SendPacket(bytes);
                }
                catch (Exception e)
                {
                    Logger.Debug(e.Message);
                }

                device.Close();
            }
        }
    }
}