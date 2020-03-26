using Autofac;
using System.Reflection;
using RegistrationService.Data.Queries;
using RegistrationService.Data.Repositories;

namespace RegistrationService.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new RegistrationQueries(QueriesConnectionString))
                .As<IRegistrationQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RegistrationRepository>()
                .As<IRegistrationRepository>()
                .InstancePerLifetimeScope();

        }
    }
}
