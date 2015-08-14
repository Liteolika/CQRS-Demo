using Contracts.Commands;
using Contracts.Events;
using Domain.Aggregates;
using Domain.CommandHandlers;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Storage;
using MassTransit;
using StructureMap;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
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
                    scan.ExcludeNamespace("Domain.Tests");
                    scan.ExcludeNamespace("Infrastructure.Tests");
                    scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommand<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IHandleEvent<>));
                });

                cfg.For<IServiceBus>().Singleton().Use("Creating servicebus", ServiceBusInitializer);
                cfg.For<ICommandBus>().Use<CommandBus>();
                cfg.For(typeof(IDomainRepository<>)).Use(typeof(EventStoreDomainRepository<>));
                cfg.For<DomainService>().Use<DomainService>();
                cfg.For<IEventStore>().Use<InMemoryEventStore>();
                cfg.For<IEventBus>().Use<EventBus>();
                //cfg.For<ViewBuilderService>().Use<ViewBuilderService>();
                
            });

            var wdih = container.WhatDoIHave();

            //var evtBus = new EventBus(container.GetInstance<IServiceBus>());
            //var store = new InMemoryEventStore(evtBus);
            //var repo = new EventStoreDomainRepository<NetworkDevice>(store);
            //var ndh = new NetworkDeviceCommandHandler(repo);

            var cmdBus = container.GetInstance<ICommandBus>();
            var domainService = container.GetInstance<DomainService>();
            //var viewbuilderService = container.GetInstance<ViewBuilderService>();

            Guid idA = Guid.NewGuid();
            Guid idB = Guid.NewGuid();
            cmdBus.SendCommand(new CreateNetworkDevice(idA, "COMPUTER1"));
            cmdBus.SendCommand(new CreateNetworkDevice(idB, "PHONE1"));

            Thread.Sleep(4000);

            for (int i = 0; i < 120; i++)
            {
                Thread.Sleep(10);
                cmdBus.SendCommand(new ChangeNetworkDevice(idA, "COMPUTER1-" + i.ToString()));
            }
            for (int i = 0; i < 120; i++)
            {
                Thread.Sleep(10);
                cmdBus.SendCommand(new ChangeNetworkDevice(idB, "PHONE1-" + i.ToString()));
            }

            
            Console.ReadKey();
            cmdBus.SendCommand(new ChangeNetworkDevice(idA, "The computer!"));
            cmdBus.SendCommand(new ChangeNetworkDevice(idB, "The phone!"));
            Console.ReadKey();

            cmdBus.SendCommand(new SetNetworkDeviceIp(idA, "172.24.180.14"));
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

    //public class NetworkDeviceViewBuilder : 
    //    IHandleEvent<NetworkDeviceCreated>,
    //    IHandleEvent<NetworkDeviceChanged>
    //{

    //    public void Handle(NetworkDeviceCreated @event)
    //    {
    //        Console.WriteLine("ViewBuilder got: {0}, {1}", @event.GetType().FullName, @event.Hostname);
    //    }

    //    public void Handle(NetworkDeviceChanged @event)
    //    {
    //        Console.WriteLine("ViewBuilder got: {0}, {1}", @event.GetType().FullName, @event.NewHostname);
    //    }
    //}

    //public class ViewBuilderService
    //{
    //    private IServiceBus _bus;
    //    private IContainer _container;

    //    public ViewBuilderService(IServiceBus bus, IContainer container)
    //    {
    //        _bus = bus;
    //        _container = container;

    //        var handlerInterfaces = GetHandlerInterfaces();

    //        foreach (var handlerInterface in handlerInterfaces)
    //        {
    //            var msgType = handlerInterface.GetGenericArguments()[0];
    //            var handlerInstance =
    //                _container.ForGenericType(typeof(IHandleEvent<>))
    //                          .WithParameters(msgType)
    //                          .GetInstanceAs<object>();

    //            var action = GetHandleAction(handlerInstance, msgType);

    //            Subscribe(_bus, msgType, action);
    //        }

    //    }

    //    private static Delegate GetHandleAction(object handlerInstance, Type msgType)
    //    {
    //        var handlerMethod = handlerInstance.GetType().GetMethod("Handle", new Type[] { msgType });

    //        var action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(msgType),
    //                                             handlerInstance, handlerMethod);
    //        return action;
    //    }

    //    private IEnumerable<Type> GetHandlerInterfaces()
    //    {
    //        var handlers = _container.Model.PluginTypes.
    //                          Where(
    //                              p =>
    //                              p.PluginType.IsGenericType &&
    //                              p.PluginType.GetGenericTypeDefinition() == typeof(IHandleEvent<>)).
    //                          Select(p => p.PluginType).ToArray();
    //        return handlers;
    //    }

    //    private static void Subscribe(IServiceBus bus, Type msgType, object action)
    //    {
    //        var subMethod = typeof(HandlerSubscriptionExtensions).GetMethods().Single(m => m.Name == "SubscribeHandler" && m.GetParameters().Length == 2).MakeGenericMethod(msgType);

    //        subMethod.Invoke(bus, new object[] { bus, action });
    //    }

    //}

    public class DomainService
    {
        private IServiceBus _bus;
        private IContainer _container;

        public DomainService(IServiceBus bus, IContainer container)
        {
            _bus = bus;
            _container = container;

            var handlerInterfaces = GetHandlerInterfaces();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var msgType = handlerInterface.GetGenericArguments()[0];
                var handlerInstance =
                    _container.ForGenericType(typeof(IHandleCommand<>))
                              .WithParameters(msgType)
                              .GetInstanceAs<object>();

                var action = GetHandleAction(handlerInstance, msgType);

                Subscribe(_bus, msgType, action);
            }

        }

        private static Delegate GetHandleAction(object handlerInstance, Type msgType)
        {
            var handlerMethod = handlerInstance.GetType().GetMethod("Handle", new Type[] { msgType});

            var action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(msgType),
                                                 handlerInstance, handlerMethod);
            return action;
        }

        private IEnumerable<Type> GetHandlerInterfaces()
        {
            return _container.Model.PluginTypes.
                              Where(
                                  p =>
                                  p.PluginType.IsGenericType &&
                                  p.PluginType.GetGenericTypeDefinition() == typeof(IHandleCommand<>)).
                              Select(p => p.PluginType).ToArray();
        }

        private static void Subscribe(IServiceBus bus, Type msgType, object action)
        {
            var subMethod = typeof(HandlerSubscriptionExtensions).GetMethods().Single(m => m.Name == "SubscribeHandler" && m.GetParameters().Length == 2).MakeGenericMethod(msgType);

            subMethod.Invoke(bus, new object[] { bus, action });
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

    public class CommandBus : ICommandBus
    {

        public IServiceBus _bus;

        public CommandBus(IServiceBus bus)
        {
            _bus = bus;
        }

        public void SendCommand<T>(T command) where T : Command
        {
            _bus.Publish<T>(command);
        }
    }

    

    

    

    

    

    

    

}
