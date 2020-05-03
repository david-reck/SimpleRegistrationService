using System;
using MediatR;

namespace RegistrationService.API.Application.Commands
{
    public class RegistrationWithMessageCommand : IRequest<bool>
    {
        public Int64 ClientId { get; private set; }

        public string FacilityCode { get; private set; }

        public string MedicalRecordNumber { get; private set; }

        public string PatientNumber { get; private set; }

        public string ADTMessage { get; private set; }

        public RegistrationWithMessageCommand(Int64 clientId, string facilityCode, string medicalRecordNumber, string patientNumber, string message)
        {
            ClientId = clientId;
            FacilityCode = facilityCode;
            MedicalRecordNumber = medicalRecordNumber;
            PatientNumber = patientNumber;
            ADTMessage = message;
        }
    }
}
