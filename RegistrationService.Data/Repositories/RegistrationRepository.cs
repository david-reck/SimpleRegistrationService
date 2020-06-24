using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using RegistrationService.Data;
using RegistrationService.Data.Domain;

namespace RegistrationService.Data.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly RegistrationContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RegistrationRepository(RegistrationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Patient Add(Patient patient)
        {
                if (patient.IsTransient())
                {
                    return _context.Patient
                        .Add(patient)
                        .Entity;
                }
                else
                {
                    return patient;
                }
        }



        public Patient Update(Patient patient)
        {
                return _context.Patient
                        .Update(patient)
                        .Entity;
        }

        public async Task<Patient> FindByIdAsync(long patientId)
        {
            var patient = await _context.Patient
                .Where(p => p.PatientId == patientId)
                .SingleOrDefaultAsync();

            return patient;
        }

        public Patient FindPatientAndPatientVisit(string medicalRecordNumber, string patientNumber)
        {
            var patient = _context.Patient.Where(p => p.MedicalRecordNumber == medicalRecordNumber).FirstOrDefault();
            if (patient != null)
            {
                var patientVisit = _context.PatientVisit.Where(pv => pv.PatientNumber == patientNumber).FirstOrDefault();
                if (patientVisit != null)
                {
                    patient.PatientVisits.Add(patientVisit);
                }

            }
            return patient;
        }
    }
}
