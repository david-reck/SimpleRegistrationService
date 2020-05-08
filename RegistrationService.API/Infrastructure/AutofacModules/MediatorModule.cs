using System.Reflection;
using Autofac;
using MediatR;
using RegistrationService.API.Application.Behaviors;
using RegistrationService.API.Application.Commands;
using RegistrationService.API.Application.DomainEventHandlers.PatientTransactionReceived;

namespace RegistrationService.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(RegistrationCommand).GetTypeInfo().Assembly);

            builder.RegisterAssemblyTypes(typeof(GetPatientDocumentCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            
            builder.RegisterAssemblyTypes(typeof(AddDocumentWhenPatientTransactionReceivedEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
