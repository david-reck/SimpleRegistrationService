using MediatR;
using RegistrationService.Data;

namespace RegistrationService.Data.Events
{
    public class PatientRegisteredEvent : INotification
    {
        public Patient patient { get; }

        public PatientRegisteredEvent(Patient registeredPatient)
        {
            patient = registeredPatient;
        }
    }
}
