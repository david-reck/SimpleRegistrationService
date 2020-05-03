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

            public Int64 FacilityId { get; set; }

            public string FacilityCode { get; set; }
            public string MedicalRecordNumber { get; set; }

            public string PatientNumber { get; set; }

            public string ADTMessage { get; set; }


        }
    }
