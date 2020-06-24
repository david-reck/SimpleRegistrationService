using RegistrationService.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using MediatR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RegistrationService.API.IntegrationEvents.Events;
using RegistrationService.API.IntegrationEvents;
using RegistrationService.Data.Domain;
using RegistrationService.API.Grpc;
using RegistrationService.Data.Repositories;
using RegistrationService.Data.Events;
using RegistrationService.Data.DTOs;

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, bool>
    {
        private readonly RegistrationContext _registrationContext;
        private readonly IMediator _mediator;
        private readonly IRegistrationIntegrationEventService _registrationIntegrationEventService;
        private IClientGRPCClientService _grpcClientService;
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationCommandHandler(RegistrationContext registrationContext,
            IMediator mediator,
            IRegistrationIntegrationEventService registrationIntegrationEventService,
            IClientGRPCClientService clientService, IRegistrationRepository registrationRepository)
        {
            _registrationContext = registrationContext ?? throw new ArgumentNullException(nameof(registrationContext));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _registrationIntegrationEventService = registrationIntegrationEventService ?? throw new ArgumentNullException(nameof(registrationIntegrationEventService));
            _grpcClientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
        }

        public async Task<bool> Handle(RegistrationCommand message, CancellationToken cancellationToken)
        {
            bool saved = false;
            Guid id = Guid.NewGuid();
            ClientFacilityDetail facilityDetails = await _grpcClientService.ClientFacilitySubscribesToModule(message.ClientId, message.adt.content.MSH.sendingFacility.namespaceId);

            var patientTransaction = new List<PatientTransaction>();
            patientTransaction.Add(new PatientTransaction { DocumentId = id });

            var patient = _registrationRepository.FindPatientAndPatientVisit(message.adt.content.PID[0].internalId[0].id,message.adt.content.PID[0].patientAccountNumber.id);
            if (patient == null)
            {
                patient = new Patient { ClientId = message.ClientId, FacilityId = facilityDetails.FacilityId, MedicalRecordNumber = message.adt.content.PID[0].internalId[0].id};
            }
            if (patient.PatientVisits.Count == 0)
            {
                var patientVist = new List<PatientVisit>();
                patientVist.Add(new PatientVisit { PatientNumber = message.adt.content.PID[0].patientAccountNumber.id, PatientTransactions = patientTransaction });
                patient.PatientVisits = patientVist;
            }
            else
            {
                patient.PatientVisits[0].PatientTransactions = patientTransaction;
            }

            if (patient.PatientId == 0)
            { _registrationContext.Add(patient); }
            else
            { _registrationContext.Update(patient); }
            saved = await _registrationContext.SaveEntitiesAsync(cancellationToken);

            message.adt.ClientId = message.ClientId;
            message.adt.FacilityId = facilityDetails.FacilityId;
            message.adt.PatientId = patient.PatientId;
            message.adt.PatientVisitId = patient.PatientVisits[0].PatientVisitId;
            message.adt.PatientTransactionId = patient.PatientVisits[0].PatientTransactions[0].PatientTransactionId;
            message.adt.id = patient.PatientVisits[0].PatientTransactions[0].DocumentId.ToString();
            var registrationReceivedEvent = new RegistrationReceivedIntegrationEvent(patient.ClientId, patient.FacilityId, patient.PatientId,
                patient.PatientVisits[0].PatientVisitId, patient.PatientVisits[0].PatientTransactions[0].DocumentId.ToString(), facilityDetails.ClientName);
            await _registrationIntegrationEventService.AddAndSaveEventAsync(registrationReceivedEvent);


            
            await _mediator.Publish(new PatientTransactionReceivedEvent(message.adt));


            return saved;
        }
    }
}
