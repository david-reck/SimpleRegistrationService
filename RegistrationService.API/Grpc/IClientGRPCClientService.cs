using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API.Grpc
{
    public interface IClientGRPCClientService
    {
        Task<Int64> ClientFacilitySubscribesToModule(Int64 clientId, string facilityCode);
    }
}
