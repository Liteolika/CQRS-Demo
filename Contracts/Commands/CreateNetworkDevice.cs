using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands
{
    public class CreateNetworkDevice : Command
    {
        public readonly Guid DeviceId;
        public readonly string Hostname;

        public CreateNetworkDevice(Guid deviceId, string hostname)
        {
            DeviceId = deviceId;
            Hostname = hostname;
        }
    }
}
