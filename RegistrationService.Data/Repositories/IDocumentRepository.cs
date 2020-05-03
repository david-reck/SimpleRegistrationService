using RegistrationService.Data.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationService.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task<bool> Add(Patient patient, string message);
    }
}
