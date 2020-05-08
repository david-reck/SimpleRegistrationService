using MediatR;
using RegistrationService.Data.DTOs;
using System;

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationCommand : IRequest<bool>
    {
        public Int64 ClientId { get; private set; }

        public Adt adt { get; private set; }

        public RegistrationCommand(Int64 clientId,  Adt message)
        {
            ClientId = clientId;
            adt = message;
        }
    }
}
