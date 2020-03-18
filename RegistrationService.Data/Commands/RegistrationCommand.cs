using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Data
{
    public class RegistrationCommand
    {
        public DateTime BirthDate { get; private set; }
        public string Gender { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }

        public RegistrationCommand(DateTime birthDate, string gender, string firstName, string middleName, string lastName)
        {
            BirthDate = birthDate;
            Gender = gender;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }
    }
}
