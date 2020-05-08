using Newtonsoft.Json;
using System;

namespace RegistrationService.Data.DTOs
{
    public class RegistrationDTO
        {
            public RegistrationDTO()
            {

            }

            [JsonProperty(PropertyName = "id")]
            public string id { get; set; }
            public Int64 ClientId { get; set; }

            public Int64 PatientId { get; set; }
            public Int64 PatientVisitId { get; set; }
            public Int64 PatientTransactionId { get; set; }
        public string ADTMessage { get; set; }


        }
    }
