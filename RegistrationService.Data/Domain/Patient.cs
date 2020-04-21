using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using RegistrationService.Data.Events;
using RegistrationService.Data.Seedwork;

namespace RegistrationService.Data
{
    public class Patient
    {
        public Patient()
        {
            PatientAddresses = new List<PatientAddress>();
            PatientAccounts = new List<PatientAccount>();
        }
        
        public Int64 PatientId { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int ClientId { get; set; }
        public Int64 FacilityId { get; set; }
        public string MedicalRecordNumber { get; set; }


        public List<PatientAddress> PatientAddresses { get; set; }

       public List<PatientAccount> PatientAccounts { get; set; }

        public bool IsTransient()
        {
            return this.PatientId == default(Int32);
        }

    }
}
