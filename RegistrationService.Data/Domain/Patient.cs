using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using RegistrationService.Data.Events;
using RegistrationService.Data.Seedwork;

namespace RegistrationService.Data.Domain
{
    public class Patient
    {
        public Patient()
        {
            PatientVisits = new List<PatientVisit>();
        }
        
        public Int64 PatientId { get; set; }
        public string MedicalRecordNumber { get; set; }
        public Int64 ClientId { get; set; }

        public Int64 FacilityId { get; set; }

       public List<PatientVisit> PatientVisits { get; set; }

        public bool IsTransient()
        {
            return this.PatientId == default(Int32);
        }

    }
}
