using Contracts.Events;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates
{
    public class NetworkDevice : AggregateRoot
    {

        public string hostname { get; private set; }
        public IPAddress ipAddress { get; private set; }

        public NetworkDevice()
        {
            RegisterTransition<NetworkDeviceCreated>(Apply);
            RegisterTransition<NetworkDeviceChanged>(Apply);
            RegisterTransition<NetworkDeviceIPChanged>(Apply);
        }

        public NetworkDevice(Guid deviceId, string hostname) : this()
        {
            ApplyChange(new NetworkDeviceCreated(deviceId, hostname));
        }

        public void SetHostname(string newHostname)
        {
            ApplyChange(new NetworkDeviceChanged(_AggregateId, newHostname));
        }

        public void SetIpV4Address(string ipv4Address)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipv4Address, out ipAddress))
            {
                ApplyChange(new NetworkDeviceIPChanged(_AggregateId, ipv4Address));
            }
            else
            {
                throw new Exception("IpAddress was not in a correct format");
            }
           
        }


        public void Apply(NetworkDeviceCreated @event)
        {
            _AggregateId = @event.DeviceId;
            hostname = @event.Hostname;
        }

        public void Apply(NetworkDeviceChanged @event)
        {
            hostname = @event.NewHostname;
        }

        public void Apply(NetworkDeviceIPChanged @event)
        {
            ipAddress = IPAddress.Parse(@event.Ipv4Address);
        }

        
    }

    


}
