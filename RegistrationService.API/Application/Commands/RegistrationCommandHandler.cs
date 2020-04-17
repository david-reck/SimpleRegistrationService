using RegistrationService.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using RegistrationService.API.IntegrationEvents.Events;
using RegistrationService.API.IntegrationEvents;

namespace RegistrationService.API.Commands
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, bool>
    {

        private readonly RegistrationContext _registrationContext;
        private readonly IMediator _mediator;
        private readonly IRegistrationIntegrationEventService  _registrationIntegrationEventService;

        public RegistrationCommandHandler(RegistrationContext registrationContext, IMediator mediator, IRegistrationIntegrationEventService registrationIntegrationEventService)
        {
            _registrationContext = registrationContext ?? throw new ArgumentNullException(nameof(registrationContext));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _registrationIntegrationEventService = registrationIntegrationEventService ?? throw new ArgumentNullException(nameof(registrationIntegrationEventService));
        }

        public async Task<bool> Handle(RegistrationCommand message, CancellationToken cancellationToken)
        {
            

            var patient = new Patient();
            patient.BirthDate = message.BirthDate;
            patient.FirstName = message.FirstName;
            patient.MiddleName = message.MiddleName;
            patient.Gender = message.Gender;
            patient.LastName = message.LastName;

            var registrationReceivedEvent = new RegistrationReceivedIntegrationEvent(1, 1, 1, "1233456789", 1, message.BirthDate, message.Gender, message.FirstName, message.MiddleName, message.LastName);
            await _registrationIntegrationEventService.AddAndSaveEventAsync(registrationReceivedEvent);

            _registrationContext.Add(patient);

          
            return await _registrationContext.SaveEntitiesAsync(cancellationToken);
        }
    }
}
