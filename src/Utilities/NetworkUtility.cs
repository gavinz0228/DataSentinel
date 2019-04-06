using System;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
namespace DataSentinel.Utilities{
    public static class NetworkUtility {
        public static bool CanPing(string host, int timeout)
        {
            Ping ping = new Ping();
            byte[] data = Encoding.ASCII.GetBytes("PingMessage");
            PingOptions options = new PingOptions(64, true);
            var reply = ping.Send(host, timeout, data, options);
            if(reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}