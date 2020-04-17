using System;
using System.Threading.Tasks;
using iPas.Infrastructure.EventBus.Events;

namespace RegistrationService.API.IntegrationEvents
{
    public interface IRegistrationIntegrationEventService
    {
        Task AddAndSaveEventAsync(IntegrationEvent evt);
        Task PublishEventsThroughEventBusAsync(Guid transactionI);
    }
}
