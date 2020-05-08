using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using RegistrationService.Data.Events;
using RegistrationService.Data.Repositories;

namespace RegistrationService.API.Application.DomainEventHandlers.PatientTransactionReceived
{
    public class AddDocumentWhenPatientTransactionReceivedEventHandler
         : INotificationHandler<PatientTransactionReceivedEvent>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILoggerFactory _logger;
        public AddDocumentWhenPatientTransactionReceivedEventHandler(IDocumentRepository documentRepository,
            ILoggerFactory logger)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientTransactionReceivedEvent notification, CancellationToken cancellationToken)
        {
            await _documentRepository.Add(notification.registrationMessage);

            _logger.CreateLogger<AddDocumentWhenPatientTransactionReceivedEventHandler>()
                .LogTrace("Patient Visit: {PatientVisitId} has successfully added PatientTransaction of Id{TransactionId}",
                    notification.registrationMessage.PatientVisitId, notification.registrationMessage.id);
        }
    }
}
