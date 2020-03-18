using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Data
{
           public interface IRepository<T>
        {
            IUnitOfWork UnitOfWork { get; }
        }
    }
