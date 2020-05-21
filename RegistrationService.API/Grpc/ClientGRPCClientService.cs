using ClientService.API.Grpc;
using Grpc.Net.Client;
using RegistrationService.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API.Grpc
{
    public class ClientGRPCClientService : IClientGRPCClientService
    {
        private string _grpcClientAddress;
        private string _moduleCode;
        public ClientGRPCClientService(string address, string module)
        {
            _grpcClientAddress = address;
            _moduleCode = module;
        }
        public Task<ClientFacilityDetail> ClientFacilitySubscribesToModule(Int64 clientId, string facilityCode)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var clientChannel = GrpcChannel.ForAddress(_grpcClientAddress);
            var clientClient = new ClientApiRetrieval.ClientApiRetrievalClient(clientChannel);
            var modulesRequest = new ModulesRequest { ClientId = clientId, FacilityCode = facilityCode };
            var clientServieReply = clientClient.FindModulesByClientIdAndFacilityCode(modulesRequest);

            if (clientServieReply != null)
            {
                return Task.FromResult(new ClientFacilityDetail { ClientId = clientServieReply.ClientId, ClientName = clientServieReply.ClientName, FacilityId = clientServieReply.FacilityId, FacilityCode = clientServieReply.FacilityCode });
            }
            return Task.FromResult(new ClientFacilityDetail());
        }
    }
}
