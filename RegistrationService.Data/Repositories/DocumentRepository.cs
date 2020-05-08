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

        public async Task<bool> Add(Adt adt)
        {
            var added = false;
            ItemResponse<Adt> item = await _container.CreateItemAsync<Adt>(adt, new PartitionKey(adt.ClientId));
            if(item !=null)
            { added = true; }
            return added;
        }
    }
}
