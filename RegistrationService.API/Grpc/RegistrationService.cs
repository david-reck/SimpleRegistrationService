using Grpc.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using RegistrationService.API.IntegrationEvents;
using RegistrationService.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API.Grpc
{
    public class RegistrationService : RegistrationApiRetrieval.RegistrationApiRetrievalBase
    {

        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(ILogger<RegistrationService> logger)
        {
            _logger = logger;
        }

        public  override async Task<AdtMessageResponse> FindAdtMessageById(AdtMessageRequest request, ServerCallContext context)
        {

            string EndpointUri = "https://pelitascosmosadmin.documents.azure.com:443/";
            // The primary key for the Azure Cosmos account.
            string PrimaryKey = "RUMdNn9Lf5QzyLOjrjn1CLPXapLmdL3sfwy8RrNje2wJVF4F5D1h9nhpwTVzvmX87QyBqWSILBCOQ6WxkoROzA==";

            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "iPas.Registration" });
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("ClientFeed");
            Container container = await database.CreateContainerIfNotExistsAsync("ADT", "/ClientId", 400);

            ItemResponse<RegistrationDTO> registration =  await container.ReadItemAsync<RegistrationDTO>(request.Id, new PartitionKey(request.ClientId));
            
            if (registration != null)
            {
                Console.WriteLine(registration.Resource.ADTMessage);
                return new AdtMessageResponse { AdtMessage = registration.Resource.ADTMessage };
            }

            context.Status = new Status(StatusCode.NotFound, $"Document with id {request.Id} does not exist");
            return null;
        }

    }
}
