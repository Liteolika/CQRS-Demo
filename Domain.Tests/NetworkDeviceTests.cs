using Contracts.Events;
using Domain.Aggregates;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    [TestFixture]
    public class NetworkDeviceTests
    {

        [Test]
        public void New_NetworkDevice_Gets_Id()
        {
            Guid deviceId = Guid.NewGuid();
            string hostname = "SESM-001";
            NetworkDevice device = new NetworkDevice(deviceId, hostname);

            Assert.AreEqual(deviceId, device.AggregateId);
        }

        [Test]
        public void New_NetworkDevice_Has_Created_Event_Uncommitted()
        {
            Guid deviceId = Guid.NewGuid();
            string hostname = "SESM-001";
            NetworkDevice device = new NetworkDevice(deviceId, hostname);

            var events = device.UncommittedEvents();

            var expectedEvent = new NetworkDeviceCreated(deviceId, hostname);
            var actualEvent = events.First() as NetworkDeviceCreated;

            Assert.AreEqual(expectedEvent.DeviceId, actualEvent.DeviceId);
            Assert.AreEqual(expectedEvent.Hostname, actualEvent.Hostname);
           
        }

        [Test]
        public void SetHostName_AppliesChange()
        {
            Guid deviceId = Guid.NewGuid();
            string hostname = "SESM-001";
            NetworkDevice device = new NetworkDevice(deviceId, hostname);
            device.SetHostname("NEW");

            Assert.AreEqual(device.hostname, "NEW");

        }

        [Test]
        public void SetDeviceIpAddress_AppliesChanges()
        {
            Guid deviceId = Guid.NewGuid();
            string hostname = "SESM-001";
            NetworkDevice device = new NetworkDevice(deviceId, hostname);
            
            string ip = "172.24.180.14";

            device.SetIpV4Address(ip);

            Assert.AreEqual(device.ipAddress.ToString(), ip);
        }

        [Test]
        public void SetDeviceIpAddress_ThrowsOnBadIp()
        {
            Guid deviceId = Guid.NewGuid();
            string hostname = "SESM-001";
            NetworkDevice device = new NetworkDevice(deviceId, hostname);

            string ip = "asdsd";

            Assert.Throws<Exception>(() =>
            {
                device.SetIpV4Address(ip);
            });

            


        }

    }

    

}
