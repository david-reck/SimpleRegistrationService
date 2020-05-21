using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using RegistrationService.Data.DTOs;
using RegistrationService.Data.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegistrationService.API.Application.Commands
{
    public class GetDocumentCommandHandler : IRequestHandler<GetPatientDocumentCommand, PatientResultByVisitID>
    {
        private string _partitionKey;
        private CosmosClient _cosmostClient;
        private Container _container;
        private IRegistrationQueries _registrationQueries;

        public GetDocumentCommandHandler(IConfiguration config, IRegistrationQueries registrationQueries)
        {
            var settings = new DocumentDBSettings();
            config.GetSection("DocumentDatabase").Bind(settings);
            _cosmostClient = new CosmosClient(settings.EndpointUri, settings.PrimaryKey, new CosmosClientOptions() { ApplicationName = settings.ApplicationName });
            _container = _cosmostClient.GetContainer(settings.DatabaseName, settings.ContainerName);
            _partitionKey = settings.PartitionKey;
            _registrationQueries = registrationQueries ?? throw new ArgumentNullException(nameof(registrationQueries));
        }

        public async Task<PatientResultByVisitID> Handle(GetPatientDocumentCommand request, CancellationToken cancellationToken)
        {
            var documentID = await _registrationQueries.GetDocumentByVisitID(request.PatientVisitID);
            if (documentID.DocumnetID != null)
            {
                ItemResponse<Adt> registration = await _container.ReadItemAsync<Adt>(documentID.DocumnetID.ToString(), new PartitionKey(request.ClientID));
                if (registration != null)
                {
                    return PatientResult(registration);
                }
            }
            throw new Exception();
        }
    private PatientResultByVisitID PatientResult(Adt searchResults)
        {
            return new PatientResultByVisitID
            {
                PatientName = searchResults.content.PID[0].patientName[0].firstName + " " + searchResults.content.PID[0].patientName[0].lastName,
                StreetAddress = searchResults.content.PID[0].address[0].streetAddress,
                City = searchResults.content.PID[0].address[0].city,
                State = searchResults.content.PID[0].address[0].stateOrProvince,
                ZIPCode = searchResults.content.PID[0].address[0].zip,
                Country = searchResults.content.PID[0].address[0].country,
                Race = searchResults.content.PID[0].race,
                Religion = searchResults.content.PID[0].religion,
                DiagnosisDescription = searchResults.content.DG1[0].diagnosisDescription,
                DiagnosisType = searchResults.content.DG1[0].diagnosisType,
                AdmissionType = searchResults.content.PV1[0].admissionType,
                AdmitSource = searchResults.content.PV1[0].admitSource,
                AdmittingDoctor = searchResults.content.PV1[0].admittingDoctor[0].firstName + " " + searchResults.content.PV1[0].admittingDoctor[0].lastName,
                AttendingDoctor = searchResults.content.PV1[0].attendingDoctor[0].firstName + " " + searchResults.content.PV1[0].attendingDoctor[0].lastName,
                PatientClass = searchResults.content.PV1[0].patientClass,
                PatientType = searchResults.content.PV1[0].patientType,
                PatientStatusCode = searchResults.content.PV1[0].PV2[0].patientStatusCode,
                PrimInsurPlanName = searchResults.content.IN1[0].planId.id + " " + searchResults.content.IN1[0].planId.text,
                PrimInsurAddress = searchResults.content.IN1[0].insuredAddress[0].streetAddress,
                PrimInsurCity = searchResults.content.IN1[0].insuredAddress[0].city,
                PrimInsurState = searchResults.content.IN1[0].insuredAddress[0].stateOrProvince,
                PrimInsurZIPCode = searchResults.content.IN1[0].insuredAddress[0].zip,
                PrimPolicyNumber = searchResults.content.IN1[0].policyNumber,
                PrimInsuredDob = searchResults.content.IN1[0].insuredDob,
                PrimInsuredName = searchResults.content.IN1[0].insuredName[0].firstName + " " + searchResults.content.IN1[0].insuredName[0].lastName,
                PrimInsuredRelationshipToPatient = searchResults.content.IN1[0].insuredRelationshipToPatient,
                PrimInsuredSex = searchResults.content.IN1[0].insuredSex
            };
        }

    }

}
