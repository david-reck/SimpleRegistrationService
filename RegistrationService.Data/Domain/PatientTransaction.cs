using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Data.Domain
{
    public class PatientTransaction
    {
        public Int64 PatientTransactionId { get; set; }
        public Guid DocumentId { get; set; }
        public Int64 PatientVisitId { get; set; }

        public bool IsTransient()
        {
            return this.PatientTransactionId == default(Int32);
        }

    }
}
