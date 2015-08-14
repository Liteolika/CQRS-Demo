using Contracts.Commands;
using Domain.Aggregates;
using Domain.CommandHandlers;
using Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    [TestFixture]
    public class NetworkDeviceCommandHandlerTests
    {

        [Test]
        public void Handle_CreateNetworkDevice_Throws_OnDuplicate_DeviceId()
        {

            var repoMock = new Mock<IDomainRepository<NetworkDevice>>();
            
            var deviceId = Guid.NewGuid();
            var hostname = "SESM-01";

            var networkDevice = new NetworkDevice(deviceId, hostname);

            repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(networkDevice);

            NetworkDeviceCommandHandler handler = new NetworkDeviceCommandHandler(repoMock.Object);

            Assert.Throws<AggregateException>(() =>
            {
                handler.Handle(new CreateNetworkDevice(deviceId, hostname));
            });

        }

        

    }

    

}
