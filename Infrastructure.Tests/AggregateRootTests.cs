using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tests
{
    [TestFixture]
    public class AggregateRootTests
    {

        [Test]
        public void Created_Aggregate_Has_Version_Zero()
        {
            TestAggregate agg = new TestAggregate();
            Assert.AreEqual(0, agg.AggregateVersion);
        }

        [Test]
        public void Applying_Two_Events_Sets_Version_Two()
        {
            TestAggregate agg = new TestAggregate();
            agg.ApplyChange(new AggregateChanged());
            agg.ApplyChange(new AggregateChanged());
            Assert.AreEqual(2, agg.AggregateVersion);
        }

        [Test]
        public void Loading_Event_Changes_The_State()
        {
            TestAggregate agg = new TestAggregate();

            var events = new List<Event>();
            events.Add(new AggregateChanged());
            events.Add(new AggregateChanged());
            events.Add(new AggregateChanged());
            events.Add(new AggregateChanged());

            agg.LoadHistory(events);

            Assert.AreEqual(4, agg.AggregateVersion);

        }

        [Test]
        public void UncommittedEvents_Can_Be_Fetched()
        {
            TestAggregate agg = new TestAggregate();

            agg.Change();
            agg.Change();
            agg.Change();

            var uncommittedEvents = agg.UncommittedEvents();

            Assert.AreEqual(3, uncommittedEvents.Count());

        }

        [Test]
        public void Cleared_Events_Are_Gone()
        {
            TestAggregate agg = new TestAggregate();
            agg.Change();
            agg.Change();
            agg.Change();
            agg.ClearUncommittedEvents();

            var uncommittedEvents = agg.UncommittedEvents();

            Assert.AreEqual(0, uncommittedEvents.Count());

        }


    }

    public class TestAggregate : AggregateRoot
    {
        public TestAggregate()
        {
            RegisterTransition<AggregateChanged>(Apply);
        }

        private void Apply(AggregateChanged c)
        {

        }

        public void Change()
        {
            ApplyChange(new AggregateChanged());
        }


    }

    public class AggregateChanged : Event
    {

    }


}
