using System;
using System.Threading.Tasks;

namespace RegistrationService.Data
{
    public interface IRegistrationRepository : IRepository<Patient>
    {
        Patient Add(Patient patient);

        void Update(Patient patient);

        Task<Patient> GetAsync(Int64 patientId);
    }
}