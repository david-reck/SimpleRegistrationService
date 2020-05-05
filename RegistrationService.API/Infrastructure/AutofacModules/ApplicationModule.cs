using Autofac;
using System.Reflection;
using RegistrationService.Data.Queries;
using RegistrationService.Data.Repositories;
using RegistrationService.API.Grpc;

namespace RegistrationService.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        public string QueriesConnectionString { get; }
        public string GRPCClientURL { get; }
        public string ModuleName { get; }
        public ApplicationModule(string qconstr, string clientURL, string moduleName)
        {
            QueriesConnectionString = qconstr;
            GRPCClientURL = clientURL;
            ModuleName = moduleName;
        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new RegistrationQueries(QueriesConnectionString))
                .As<IRegistrationQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RegistrationRepository>()
                .As<IRegistrationRepository>()
                .InstancePerLifetimeScope();

            builder.Register(c => new ClientGRPCClientService(GRPCClientURL, ModuleName))
                .As<IClientGRPCClientService>()
                .InstancePerLifetimeScope();

 

        }
    }
}
