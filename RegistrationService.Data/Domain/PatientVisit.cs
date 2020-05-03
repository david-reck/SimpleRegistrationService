using System;
using System.Collections.Generic;
using System.Text;
using RegistrationService.Data.Seedwork;

namespace RegistrationService.Data.Domain
{
    public class PatientVisit
    {

		public PatientVisit()
		{
			PatientTransactions = new List<PatientTransaction>();
		}

		public Int64 PatientVisitId { get; set; }
		public string PatientNumber { get; set; }
		public Int64 PatientId { get; set; }

		public List<PatientTransaction> PatientTransactions { get; set; }

		public bool IsTransient()
		{
			return this.PatientVisitId == default(Int32);
		}

	}
}
