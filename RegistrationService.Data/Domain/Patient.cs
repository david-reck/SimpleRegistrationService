using System;
using System.Collections.Generic;
using System.Text;
using RegistrationService.Data.Events;

namespace RegistrationService.Data
{
    public class Patient
    {
        public Patient()
        {
            PatientAddresses = new List<PatientAddress>();
        }

        public Int64 PatientId { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public List<PatientAddress> PatientAddresses { get; set; }

      
    }
}
