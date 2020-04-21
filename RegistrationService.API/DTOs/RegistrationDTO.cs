using Newtonsoft.Json;
using System;

namespace RegistrationService.API
{
    public class RegistrationDTO
    {
        public RegistrationDTO()
        {
            id = Guid.NewGuid().ToString();
        }
        
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public int ClientId { get; set; }

        public string FacilityCode { get; set; }

        public string MedicalRecordNumber { get; set; }

        public string AccountNumber { get; set; }

        public string ADTMessage { get; set; }


    }
}
