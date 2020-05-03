using MediatR;
using RegistrationService.Data;
using RegistrationService.Data.Domain;

namespace RegistrationService.Data.Events
{
    public class PatientTransactionReceivedEvent : INotification
    {
        public Patient patient { get; }
        public string registrationMessage { get; }

        public PatientTransactionReceivedEvent(Patient registeredPatient, string message)
        {
            patient = registeredPatient;
            registrationMessage = message;
        }
    }
}
