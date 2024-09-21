using System;
using System.Net.Sockets;
using System.Net;
    
    internal struct TimeVar
    {
        internal static DateTime GetNetworkTime()
        {
            const string ntpServer = "time.windows.com";
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
            IPEndPoint ipEndPoint = new(addresses[0], 123);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            ulong intPart = ((ulong)ntpData[40] << 24) | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
            ulong fractPart = ((ulong)ntpData[44] << 24) | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

            ulong milliseconds = (intPart * 1000) + (fractPart * 1000 / 0x100000000L);
            DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }
    } 

