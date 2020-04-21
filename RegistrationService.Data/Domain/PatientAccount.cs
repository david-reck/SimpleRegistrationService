using System;
using System.Collections.Generic;
using System.Text;
using RegistrationService.Data.Seedwork;

namespace RegistrationService.Data
{
    public class PatientAccount
    {
		public Int64 PatientAccountId { get; set; }
		public string AccountNumber { get; set; }
		public Int64 PatientId { get; set; }
	}
}
