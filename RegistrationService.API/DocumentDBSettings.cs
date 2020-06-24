using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationService.API
{
    public class DocumentDBSettings
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }

        public string ApplicationName { get; set; }

        public string DatabaseName { get; set; }

        public string ContainerName { get; set; }

        public string PartitionKey { get; set; }

    }
}
