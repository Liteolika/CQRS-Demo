using Contracts.Events;
using Infrastructure;
using MassTransit;
using StructureMap;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewBuilderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory();
                    scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommand<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IHandleEvent<>));
                });

                cfg.For<IServiceBus>().Singleton().Use("Creating servicebus", ServiceBusInitializer);
                //cfg.For<IEventBus>().Use<EventBus>();
                cfg.For<ViewBuilderService>().Use<ViewBuilderService>();

            });

            
            var viewbuilderService = container.GetInstance<ViewBuilderService>();

            
            Console.ReadKey();
        }

        private static IServiceBus ServiceBusInitializer()
        {
            var bus = ServiceBusFactory.New(cfg =>
            {
                cfg.ReceiveFrom("rabbitmq://140.150.92.206/cqrs-demo");
                cfg.UseRabbitMq(f =>
                {
                    f.ConfigureHost(new Uri("rabbitmq://140.150.92.206/cqrs-demo"), c =>
                    {
                        c.SetUsername("petcar");
                        c.SetPassword("?!Krone2009");
                    });
                });
            });

            return bus;
        }
    }



    public class EventBus : IEventBus
    {

        public IServiceBus _bus;

        public EventBus(IServiceBus bus)
        {
            _bus = bus;
        }

        public void PublishEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                var eventType = @event.GetType();
                _bus.Publish(@event, eventType);
            }
        }
    }

    public class NetworkDeviceViewBuilder :
        IHandleEvent<NetworkDeviceCreated>,
        IHandleEvent<NetworkDeviceChanged>,
        IHandleEvent<NetworkDeviceIPChanged>
    {

        public void Handle(NetworkDeviceCreated @event)
        {
            Console.WriteLine("ViewBuilder got: {0}, {1}", @event.GetType().FullName, @event.Hostname);
        }

        public void Handle(NetworkDeviceChanged @event)
        {
            Console.WriteLine("ViewBuilder got: {0}, {1}", @event.GetType().FullName, @event.NewHostname);
        }

        public void Handle(NetworkDeviceIPChanged @event)
        {
            Console.WriteLine("ViewBuilder got: {0}, {1}", @event.GetType().FullName, @event.Ipv4Address.ToString());
        }
    }

    public class ViewBuilderService
    {
        private IServiceBus _bus;
        private IContainer _container;

        public ViewBuilderService(IServiceBus bus, IContainer container)
        {
            _bus = bus;
            _container = container;

            var handlerInterfaces = GetHandlerInterfaces();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var msgType = handlerInterface.GetGenericArguments()[0];
                var handlerInstance =
                    _container.ForGenericType(typeof(IHandleEvent<>))
                              .WithParameters(msgType)
                              .GetInstanceAs<object>();

                var action = GetHandleAction(handlerInstance, msgType);

                Subscribe(_bus, msgType, action);
            }

        }

        private static Delegate GetHandleAction(object handlerInstance, Type msgType)
        {
            var handlerMethod = handlerInstance.GetType().GetMethod("Handle", new Type[] { msgType });

            var action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(msgType),
                                                 handlerInstance, handlerMethod);
            return action;
        }

        private IEnumerable<Type> GetHandlerInterfaces()
        {
            var handlers = _container.Model.PluginTypes.
                              Where(
                                  p =>
                                  p.PluginType.IsGenericType &&
                                  p.PluginType.GetGenericTypeDefinition() == typeof(IHandleEvent<>)).
                              Select(p => p.PluginType).ToArray();
            return handlers;
        }

        private static void Subscribe(IServiceBus bus, Type msgType, object action)
        {
            var subMethod = typeof(HandlerSubscriptionExtensions).GetMethods().Single(m => m.Name == "SubscribeHandler" && m.GetParameters().Length == 2).MakeGenericMethod(msgType);

            subMethod.Invoke(bus, new object[] { bus, action });
        }

    }


}
