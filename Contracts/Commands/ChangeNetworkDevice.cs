using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands
{
    public class ChangeNetworkDevice : Command
    {
        public readonly Guid DeviceId;
        public readonly string NewHostname;

        public ChangeNetworkDevice(Guid deviceId, string newHostname)
        {
            DeviceId = deviceId;
            NewHostname = newHostname;
        }
    }
}
