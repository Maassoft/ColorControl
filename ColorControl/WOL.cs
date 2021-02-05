using NLog;
using System;
using PacketDotNet;
using System.Net.NetworkInformation;
using SharpPcap;
using SharpPcap.Npcap;
using System.Net;

namespace ColorControl
{
    class WOL
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void WakeFunction2(string macAddress)
        {
            //PhysicalAddress.Parse(macAddress).SendWol();
        }

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

                if (device is NpcapDevice npcapDevice)
                {
                    Logger.Debug($"Opening network device (NpcapDevice): FriendlyName: {npcapDevice.Interface.FriendlyName}, Description: {npcapDevice.Interface.Description}");
                }
                else
                {
                    Logger.Debug($"Opening network device (NOT A NpcapDevice!): {device.Name}, Description: {device.Description}");
                }

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