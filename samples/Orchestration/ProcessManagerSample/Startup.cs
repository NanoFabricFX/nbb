﻿using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBB.Core.Abstractions;
using NBB.EventStore;
using NBB.EventStore.AdoNet;
using NBB.Messaging.Host;
using NBB.Messaging.Host.Builder;
using NBB.Messaging.Host.MessagingPipeline;
using NBB.Messaging.InProcessMessaging.Extensions;
using NBB.ProcessManager.Definition;
using NBB.ProcessManager.Runtime;
using NBB.ProcessManager.Runtime.Timeouts;
using NBB.Resiliency;
using ProcessManagerSample.MessageMiddlewares;

namespace ProcessManagerSample
{
    public class Startup
    {
        public static void ConfigureServicesDelegate(HostBuilderContext context, IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetEntryAssembly());
            //services.AddNatsMessaging();
            services.AddInProcessMessaging();

            services.AddMediatR(Enumerable.Empty<Assembly>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped<INotificationHandler<TimeoutOccured>, TimeoutOccuredHandler>();

            services.AddProcessManagerDefinition();
            services.AddProcessManagerRuntime();
            services.AddNotificationHandlers(typeof(ProcessManagerNotificationHandler<,,>));

            services.AddEventStore()
                .WithNewtownsoftJsonEventStoreSeserializer()
                .WithAdoNetEventRepository();

            services.AddResiliency();
            services.AddMessagingHost()
                .AddSubscriberServices(config => config
                    .FromMediatRHandledCommands().AddClassesAssignableTo<ICommand>()
                    .FromMediatRHandledEvents().AddAllClasses())
                .WithDefaultOptions()
                .UsePipeline(builder => builder
                    .UseCorrelationMiddleware()
                    .UseExceptionHandlingMiddleware()
                    //.UseMiddleware<OpenTracingMiddleware>()
                    .UseDefaultResiliencyMiddleware()
                    .UseMiddleware<SubscriberLoggingMiddleware>()
                    .UseMediatRMiddleware());
        }
    }
}