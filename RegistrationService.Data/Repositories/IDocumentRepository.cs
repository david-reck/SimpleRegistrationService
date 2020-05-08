using RegistrationService.Data.Domain;
using RegistrationService.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationService.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task<bool> Add(Adt adt);

    }
}
