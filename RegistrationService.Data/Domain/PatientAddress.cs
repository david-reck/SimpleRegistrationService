﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Data
{
	public class PatientAddress
	{
		public Int64 PatientAddressId { get; set; }
		public string StreetAddress { get; set; }
		public string StreetAddress2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public Int64 PatientId { get; set; }

	}
}