using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class NetworkDeviceCreated : Event
    {
        public readonly Guid DeviceId;
        public readonly string Hostname;

        public NetworkDeviceCreated(Guid deviceId, string hostname)
        {
            DeviceId = deviceId;
            Hostname = hostname;
        }
    }
}
