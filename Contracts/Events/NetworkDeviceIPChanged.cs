using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class NetworkDeviceIPChanged : Event
    {
        public readonly Guid DeviceId;
        public readonly string Ipv4Address;

        public NetworkDeviceIPChanged(Guid deviceId, string ipv4Address)
        {
            this.DeviceId = deviceId;
            this.Ipv4Address = ipv4Address;
        }

    }
}
