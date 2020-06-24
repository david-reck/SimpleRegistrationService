using RegistrationService.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegistrationService.Data.Queries
{
    public interface IRegistrationQueries
    {
        Task<PatientDetail> GetPatientByAccountNumAsync(Int64 patientId);

        Task<IEnumerable<RegistrationSummary>> GetRegistrationsModifiedAfterAsync(DateTime modifiedAfter);

        Task<DocumentResult> GetDocumentByVisitID(int VisitID);


    }
}
