using System;
using MediatR;

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationWithMessageCommand : IRequest<bool>
    {
        public int ClientId { get; private set; }

        public string FacilityCode { get; private set; }

        public string MedicalRecordNumber { get; private set; }

        public string AccountNumber { get; private set; }

        public string ADTMessage { get; private set; }

        public RegistrationWithMessageCommand(int clientId, string facilityCode, string medicalRecordNumber, string accountNumber, string message)
        {
            ClientId = clientId;
            FacilityCode = facilityCode;
            MedicalRecordNumber = medicalRecordNumber;
            AccountNumber = accountNumber;
            ADTMessage = message;
        }
    }
}
