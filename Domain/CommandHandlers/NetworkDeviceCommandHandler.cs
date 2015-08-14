using Contracts.Commands;
using Domain.Aggregates;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CommandHandlers
{
    public class NetworkDeviceCommandHandler :
        IHandleCommand<CreateNetworkDevice>,
        IHandleCommand<ChangeNetworkDevice>,
        IHandleCommand<SetNetworkDeviceIp>
    {

        private IDomainRepository<NetworkDevice> _repo;

        public NetworkDeviceCommandHandler(IDomainRepository<NetworkDevice> repo)
        {
            _repo = repo;
        }

        public void Handle(CreateNetworkDevice command)
        {
            var device = _repo.GetById(command.DeviceId);
            if (device != null)
                throw new AggregateException("networkdevice already exists");
            device = new NetworkDevice(command.DeviceId, command.Hostname);
            _repo.Store(device);
            Console.WriteLine("Created network device with name {0}. Version {1}", command.Hostname, device.AggregateVersion);
        }

        public void Handle(ChangeNetworkDevice command)
        {
            var device = _repo.GetById(command.DeviceId);
            if (device == null)
                throw new AggregateException("network device does not exist");
            device.SetHostname(command.NewHostname);
            _repo.Store(device);
            Console.WriteLine("Changed network device to name {0}. Version {1}", command.NewHostname, device.AggregateVersion);
        }

        public void Handle(SetNetworkDeviceIp command)
        {
            var device = _repo.GetById(command.DeviceId);
            if (device == null)
                throw new AggregateException("network device does not exist");
            device.SetIpV4Address(command.Ipv4Address);
            _repo.Store(device);
            Console.WriteLine("Changed network device ipaddress to {0}. Version {1}", command.Ipv4Address, device.AggregateVersion);
        }
    }
}
