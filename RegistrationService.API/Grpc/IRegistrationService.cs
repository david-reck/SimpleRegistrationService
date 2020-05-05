using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API.Grpc
{
    public interface IRegistrationService
    {
        Task<AdtMessageResponse> FindAdtMessageById(AdtMessageRequest request, ServerCallContext context);

        Task<SearchAPIAdtMessageResponse> SearchAPIFindAdtMessageById(SearchAPIAdtMessageRequest request, ServerCallContext context);
    }
}
