using System;
using System.Threading.Tasks;

namespace RegistrationService.Data.Repositories
{
    public interface IRegistrationRepository : IRepository<Patient>
    {
        Patient Add(Patient patient);

        Patient Update(Patient patient);

        Task<Patient> FindByIdAsync(Int64 patientId);
    }
}