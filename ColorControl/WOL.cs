using NLog;
using System;
using PacketDotNet;
using System.Net.NetworkInformation;
using SharpPcap;
using SharpPcap.Npcap;
using System.Net;
using System.Net.Sockets;

namespace ColorControl
{
    class WOL
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const string ArgumentExceptionInvalidMacAddressLength = "Invalid MAC address length.";
        private const string ArgumentExceptionInvalidPasswordLength = "Invalid password length.";
        private const int DefaultWolPort = 9;

        public static void WakeFunction(string macAddress, bool usePcap = false)
        {
            if (usePcap)
            {
                WakeFunctionPcap(macAddress);
            }
            else
            {
                WakeFunctionToAllNics(macAddress);
            }
        }

        public static void WakeFunctionToAllNics(string macAddress)
        {
            try
            {
                var address = PhysicalAddress.Parse(macAddress);
                var addressBytes = address.GetAddressBytes();
                var data = GetWolPacket(addressBytes);

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    Logger.Debug($"Found network: {ni.Name} ({ni.Description})");
                    try
                    {
                        if (/*ni.OperationalStatus == OperationalStatus.Up &&*/ ni.SupportsMulticast && ni.GetIPProperties().GetIPv4Properties() != null)
                        {
                            var id = ni.GetIPProperties().GetIPv4Properties().Index;
                            if (NetworkInterface.LoopbackInterfaceIndex != id)
                            {
                                foreach (var uip in ni.GetIPProperties().UnicastAddresses)
                                {
                                    if (uip.Address.AddressFamily == AddressFamily.InterNetwork)
                                    {
                                        Logger.Debug($"Broadcast WOL in network: {ni.Name} ({ni.Description}), local address: {uip.Address}");
                                        var local = new IPEndPoint(uip.Address, 0);
                                        var udpc = new UdpClient(local);
                                        udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                        udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                                        var target = new IPEndPoint(IPAddress.Broadcast, DefaultWolPort);
                                        udpc.Send(data, data.Length, target);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"WakeFunctionToAllNics: while sending to specific network: {ni.Name} ({ni.Description}): {ex.ToLogString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WakeFunctionToAllNics: {ex.ToLogString()}");
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="password"/> is not 0 or 6.</exception>
        private static byte[] GetWolPacket(byte[] macAddress, byte[] password = null)
        {
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));
            if (macAddress.Length != 6)
                throw new ArgumentException(ArgumentExceptionInvalidMacAddressLength);

            password = password ?? new byte[0];
            if (password.Length != 0 && password.Length != 6)
                throw new ArgumentException(ArgumentExceptionInvalidPasswordLength);

            var packet = new byte[17 * 6 + password.Length];

            int offset, i;
            for (offset = 0; offset < 6; ++offset)
                packet[offset] = 0xFF;

            for (offset = 6; offset < 17 * 6; offset += 6)
                for (i = 0; i < 6; ++i)
                    packet[i + offset] = macAddress[i];

            if (password.Length > 0)
            {
                for (offset = 16 * 6 + 6; offset < (17 * 6 + password.Length); offset += 6)
                    for (i = 0; i < 6; ++i)
                        packet[i + offset] = password[i];
            }
            return packet;
        }


        //You need SharpPcap for this to work
        private static void WakeFunctionPcap(string macAddress)
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