using RegistrationService.Data.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RegistrationService.Data.DTOs;
using Microsoft.Extensions.Logging;

namespace RegistrationService.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private string _dbName;
        private string _containerName;
        private string _partitionKey;
        private CosmosClient _cosmostClient;
        private Container _container;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository( string endPointURL, string primaryKey, string applicationName, 
            string dbName, string containerName, string partitionKey, ILogger<DocumentRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cosmostClient = new CosmosClient(endPointURL, primaryKey, new CosmosClientOptions() { ApplicationName = applicationName });
            _container = _cosmostClient.GetContainer(dbName, containerName);
        }

        public async Task<bool> Add(Patient patient, string message)
        {
            var added = false;
            RegistrationDTO registrationDTO = new RegistrationDTO();
            registrationDTO.ADTMessage = message;
            registrationDTO.PatientNumber = patient.PatientVisits[0].PatientNumber;
            registrationDTO.ClientId = patient.ClientId;
            registrationDTO.FacilityId = patient.FacilityId;
            registrationDTO.id = patient.PatientVisits[0].PatientTransactions[0].DocumentId.ToString();
            ItemResponse<RegistrationDTO> item = await _container.CreateItemAsync<RegistrationDTO>(registrationDTO, new PartitionKey(patient.ClientId));
            if(item !=null)
            { added = true; }
            return added;
        }
    }
}
