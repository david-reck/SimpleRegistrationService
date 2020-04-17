using System;
using iPas.Infrastructure.EventBus.Events;

namespace RegistrationService.API.IntegrationEvents.Events
{
    public class RegistrationReceivedIntegrationEvent : IntegrationEvent
    {
        public int ClientId { get; set; }
        public int FacilityId { get; set; }
        public Int64 EpisodeOfCareId { get; set; }
        public string MedicalRecordNumber { get; set; }
        public Int64 TransactionId { get; set; }

        //******************
        //TO BE REMOVED AFTER ADDING GRPC INTEGRATION
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public RegistrationReceivedIntegrationEvent(int clientId, int facilityId, Int64 episodeOfCareId, string medicalRecordNumber, Int64 transactionId, 
                DateTime birthDate, string gender, string firstName, string middleName, string lastName)
        {
            ClientId = clientId;
            FacilityId = facilityId;
            EpisodeOfCareId = episodeOfCareId;
            MedicalRecordNumber = medicalRecordNumber;
            TransactionId = transactionId;
            BirthDate = birthDate;
            Gender = gender;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }
    }
}
