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

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationWithMessageCommandHandler : IRequestHandler<RegistrationWithMessageCommand, bool>
    {
        private readonly RegistrationContext _registrationContext;
        private readonly IMediator _mediator;
        private readonly IRegistrationIntegrationEventService _registrationIntegrationEventService;

        public RegistrationWithMessageCommandHandler(RegistrationContext registrationContext, IMediator mediator, IRegistrationIntegrationEventService registrationIntegrationEventService)
        {
            _registrationContext = registrationContext ?? throw new ArgumentNullException(nameof(registrationContext));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _registrationIntegrationEventService = registrationIntegrationEventService ?? throw new ArgumentNullException(nameof(registrationIntegrationEventService));
        }

        public async Task<bool> Handle(RegistrationWithMessageCommand message, CancellationToken cancellationToken)
        {
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(message.ADTMessage);

            var jsonString = JsonConvert.SerializeXmlNode(xmldoc);
            Hl7Adt hl7Adt = JsonConvert.DeserializeObject<Hl7Adt>(jsonString);

            var patientAccounts = new List<PatientAccount>();
            patientAccounts.Add(new PatientAccount { AccountNumber = message.AccountNumber });

            var patient = new Patient();
            patient.PatientAccounts = patientAccounts;
            patient.MedicalRecordNumber = message.MedicalRecordNumber;
            patient.ClientId = message.ClientId;
            patient.FirstName = hl7Adt.Hl7Message.Pid.Pid5.Pid51;
            patient.LastName = hl7Adt.Hl7Message.Pid.Pid5.Pid52;
            patient.Gender = hl7Adt.Hl7Message.Pid.Pid8.Pid81;

            string[] format = { "yyyyMMdd" };
            DateTime date;

            DateTime.TryParseExact(hl7Adt.Hl7Message.Pid.Pid7.Pid71,
                                       format,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out date);
            patient.BirthDate = date;

            //TODO:  Look Up Facility ID from Faclity Code and ClientId -- Hard Code for now
            patient.FacilityId = 1;

            _registrationContext.Add(patient);
            
            

            RegistrationDTO registrationDTO = new RegistrationDTO();
            registrationDTO.ADTMessage = jsonString;
            registrationDTO.AccountNumber = message.AccountNumber;
            registrationDTO.ClientId = message.ClientId;
            registrationDTO.FacilityCode = message.FacilityCode;
           //*****Cosmos Activity
            string EndpointUri = "https://pelitascosmosadmin.documents.azure.com:443/";
            // The primary key for the Azure Cosmos account.
            string PrimaryKey = "RUMdNn9Lf5QzyLOjrjn1CLPXapLmdL3sfwy8RrNje2wJVF4F5D1h9nhpwTVzvmX87QyBqWSILBCOQ6WxkoROzA==";

            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "iPas.Registration" });
            Database database  = await cosmosClient.CreateDatabaseIfNotExistsAsync("ClientFeed");
            Container container = await database.CreateContainerIfNotExistsAsync("ADT", "/ClientId", 400);

           ItemResponse<RegistrationDTO> item = await container.CreateItemAsync<RegistrationDTO>(registrationDTO, new PartitionKey(message.ClientId));


            var registrationReceivedEvent = new RegistrationReceivedIntegrationEvent(message.ClientId,  1, message.AccountNumber, message.MedicalRecordNumber ,registrationDTO.id,
                    patient.BirthDate, patient.Gender, patient.LastName, "", patient.LastName);
            await _registrationIntegrationEventService.AddAndSaveEventAsync(registrationReceivedEvent);




            return await _registrationContext.SaveEntitiesAsync(cancellationToken);
        }
    }
}
