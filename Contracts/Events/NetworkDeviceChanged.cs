using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class NetworkDeviceChanged : Event
    {
        public readonly Guid DeviceId;
        public readonly string NewHostname;

        public NetworkDeviceChanged(Guid deviceId, string newHostname)
        {
            DeviceId = deviceId;
            NewHostname = newHostname;
        }
    }
}
