using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RegistrationService.Data
{
    public class RegistrationCommandHandler
    {
        private readonly RegistrationContext _registrationContext;

        public RegistrationCommandHandler(RegistrationContext registrationContext)
        {
            _registrationContext = registrationContext ?? throw new ArgumentNullException(nameof(registrationContext));
        }

        public async Task<bool> Handle(RegistrationCommand message, CancellationToken cancellationToken)
        {
            
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            var patient = new Patient();
            patient.BirthDate = message.BirthDate;
            patient.FirstName = message.FirstName;
            patient.MiddleName = message.MiddleName;
            patient.LastName = message.LastName;

            _registrationContext.Add(patient);

            return await _registrationContext.SaveEntitiesAsync(cancellationToken);
        }
    }
}
