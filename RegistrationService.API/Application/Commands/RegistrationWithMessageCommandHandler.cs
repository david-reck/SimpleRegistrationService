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
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.Azure.Cosmos;
using RegistrationService.API.DTORaw;
using RegistrationService.Data.Domain;
using RegistrationService.API.Grpc;
using System.Linq;
using RegistrationService.Data.Repositories;
using RegistrationService.Data.Events;

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationWithMessageCommandHandler : IRequestHandler<RegistrationWithMessageCommand, bool>
    {
        private readonly RegistrationContext _registrationContext;
        private readonly IMediator _mediator;
        private readonly IRegistrationIntegrationEventService _registrationIntegrationEventService;
        private IClientGRPCClientService _grpcClientService;
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationWithMessageCommandHandler(RegistrationContext registrationContext,
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

        public async Task<bool> Handle(RegistrationWithMessageCommand message, CancellationToken cancellationToken)
        {
            bool saved = false;
            Guid id = Guid.NewGuid();
            Int64 facilityId = await _grpcClientService.ClientFacilitySubscribesToModule(message.ClientId, message.FacilityCode);

            var patientTransaction = new List<PatientTransaction>();
            patientTransaction.Add(new PatientTransaction { DocumentId = id });

            var patient = _registrationRepository.FindPatientAndPatientVisit(message.MedicalRecordNumber, message.PatientNumber);
            if(patient == null)
            {
                patient = new Patient { ClientId = message.ClientId, FacilityId = facilityId, MedicalRecordNumber = message.MedicalRecordNumber };
            }            
            if (patient.PatientVisits.Count == 0)
            {
                var patientVist = new List<PatientVisit>();
                patientVist.Add(new PatientVisit { PatientNumber = message.PatientNumber, PatientTransactions = patientTransaction});
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

            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(message.ADTMessage);

            var jsonString = JsonConvert.SerializeXmlNode(xmldoc);
            Hl7Adt hl7Adt = JsonConvert.DeserializeObject<Hl7Adt>(jsonString);

            var registrationReceivedEvent = new RegistrationReceivedIntegrationEvent(patient.ClientId, patient.FacilityId, patient.PatientId,
                patient.PatientVisits[0].PatientVisitId, patient.PatientVisits[0].PatientTransactions[0].DocumentId.ToString());
            await _registrationIntegrationEventService.AddAndSaveEventAsync(registrationReceivedEvent);


            saved =  await _registrationContext.SaveEntitiesAsync(cancellationToken);
            await _mediator.Publish(new PatientTransactionReceivedEvent(patient, jsonString));
            
            
            return saved;
        }
    }
}
