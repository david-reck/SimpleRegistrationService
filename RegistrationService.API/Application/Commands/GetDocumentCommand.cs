using MediatR;
using RegistrationService.Data.DTOs;
using System;

namespace RegistrationService.API.Application.Commands
{
    public class GetPatientDocumentCommand : IRequest<PatientResultByVisitID>
    {
        public GetPatientDocumentCommand(int _patientVisitID, Int64 _clientID)
        {
            this.PatientVisitID = _patientVisitID;
            this.ClientID = _clientID;
        }
        public int PatientVisitID { get; set; }

        public Int64 ClientID { get; set; }
    }

}
