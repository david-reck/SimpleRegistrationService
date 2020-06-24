using Grpc.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegistrationService.API.IntegrationEvents;
using RegistrationService.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API.Grpc
{
    public class RegistrationService : RegistrationApiRetrieval.RegistrationApiRetrievalBase, IRegistrationService
    {
        private string _partitionKey;
        private CosmosClient _cosmostClient;
        private Container _container;

        public RegistrationService(IConfiguration config)
        {
            var settings = new DocumentDBSettings();
            config.GetSection("DocumentDatabase").Bind(settings);
            _cosmostClient = new CosmosClient(settings.EndpointUri, settings.PrimaryKey, new CosmosClientOptions() { ApplicationName = settings.ApplicationName });
            _container = _cosmostClient.GetContainer(settings.DatabaseName, settings.ContainerName);
            _partitionKey = settings.PartitionKey;
        }

        public  override async Task<AdtMessageResponse> FindAdtMessageById(AdtMessageRequest request, ServerCallContext context)
        {
            ItemResponse<Adt> registration =  await _container.ReadItemAsync<Adt>(request.Id, new PartitionKey(request.ClientId));
            
            if (registration != null)
            {
                return new AdtMessageResponse { AdtMessage = JsonConvert.SerializeObject(registration.Resource) };
            }

            context.Status = new Status(StatusCode.NotFound, $"Document with id {request.Id} does not exist");
            return null;
        }

        public override async Task<SearchAPIAdtMessageResponse> SearchAPIFindAdtMessageById(SearchAPIAdtMessageRequest request, ServerCallContext context)
        {
            ItemResponse<Adt> registration = await _container.ReadItemAsync<Adt>(request.Id, new PartitionKey(request.ClientId));
            if (registration != null)
            {
                return new SearchAPIAdtMessageResponse { AdtMessage = JsonConvert.SerializeObject(registration.Resource) };
            }

            context.Status = new Status(StatusCode.NotFound, $"Document with id {request.Id} does not exist");
            return null;
        }

    }
        }
