using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using iPas.Infrastructure.EventBus.Abstractions;
using iPas.Infrastructure.EventBus.Events;
using iPas.Infrastructure.IntegrationEventLogEF;
using iPas.Infrastructure.IntegrationEventLogEF.Services;
using RegistrationService.Data;

namespace RegistrationService.API.IntegrationEvents
{
    public class RegistrationIntegrationEventService : IRegistrationIntegrationEventService
    {

        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly RegistrationContext _registrationContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<RegistrationIntegrationEventService> _logger;

        public RegistrationIntegrationEventService(IEventBus eventBus,
            RegistrationContext registrationContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<RegistrationIntegrationEventService> logger)
        {
            _registrationContext = registrationContext ?? throw new ArgumentNullException(nameof(registrationContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_registrationContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    var eventLog = logEvt.DeserializeJsonContent(Type.GetType(logEvt.EventTypeName));
                    _eventBus.Publish(eventLog.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _registrationContext.GetCurrentTransaction());
        }
    }
}
