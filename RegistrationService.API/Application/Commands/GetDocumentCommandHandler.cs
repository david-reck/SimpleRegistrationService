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
    public class GetDocumentCommandHandler : IRequestHandler<GetPatientDocumentCommand, DocumentResult>
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

        public async Task<DocumentResult> Handle(GetPatientDocumentCommand request, CancellationToken cancellationToken)
        {
            var documentID = await _registrationQueries.GetDocumentByVisitID(request.PatientVisitID);
            if (documentID.DocumnetID != null)
            {
                ItemResponse<RegistrationDTO> registration = await _container.ReadItemAsync<RegistrationDTO>(documentID.DocumnetID.ToString(), new PartitionKey(request.ClientID));
            }

            return documentID;
        }
    }
}
