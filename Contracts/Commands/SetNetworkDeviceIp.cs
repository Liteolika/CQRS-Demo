using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands
{
    public class SetNetworkDeviceIp : Command
    {
        public readonly Guid DeviceId;
        public readonly string Ipv4Address;

        public SetNetworkDeviceIp(Guid deviceId, string ipv4Address)
        {
            DeviceId = deviceId;
            Ipv4Address = ipv4Address;
        }

    }
}
