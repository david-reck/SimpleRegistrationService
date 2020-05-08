using MediatR;
using RegistrationService.Data;
using RegistrationService.Data.Domain;
using RegistrationService.Data.DTOs;

namespace RegistrationService.Data.Events
{
    public class PatientTransactionReceivedEvent : INotification
    {
        public Adt registrationMessage { get; }

        public PatientTransactionReceivedEvent(Adt message)
        {
            registrationMessage = message;
        }
    }
}
