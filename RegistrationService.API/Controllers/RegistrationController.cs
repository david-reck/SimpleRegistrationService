using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Data;
using RegistrationService.Data.Queries;
using MediatR;
using RegistrationService.API.Application.Commands;
using RegistrationService.Data.Domain;
using RegistrationService.Data.DTOs;

namespace RegistrationService.API.Controllers
{
    [Route("api/registrations")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private RegistrationContext _context;
        private IRegistrationQueries _registrationQueries;
        private IMediator _mediatr;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(RegistrationContext context, IRegistrationQueries registrationQueries, IMediator mediatr, ILogger<RegistrationController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _registrationQueries = registrationQueries ?? throw new ArgumentNullException(nameof(registrationQueries));
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        // GET: api/Registration
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patient.ToListAsync();
        }

        [Route("GetPatientsDetailByID/{PatientVisitID:long}")]
        [HttpGet]
        public async Task<ActionResult<DocumentResult>> GetPatientsDetailByID(long PatientVisitID)
        {

            var Id = Convert.ToInt32(PatientVisitID);
            var command = new GetPatientDocumentCommand(Id, 1);
            _logger.LogInformation("-----Sending command: GetPatientDocumentCommand");

            var result = await _mediatr.Send(command);

            return Ok(result);
        }

        [Route("registrationsafterlastupdate/{modifiedAfter:datetime}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegistrationSummary>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<RegistrationSummary>>> GetRegistrationsModifiedAfterAsync(DateTime modifiedAfter)
        {
            var registrationSummaries = await _registrationQueries.GetRegistrationsModifiedAfterAsync(modifiedAfter);

            return Ok(registrationSummaries);
        }

        [Route("patientbyaccountnumber/{patientId:long}")]
        [HttpGet]
        public async Task<ActionResult<PatientDetail>> GetPatientByAccountNumAsync(long patientId)
        {
            var patientDetail = await _registrationQueries.GetPatientByAccountNumAsync(patientId);

            if (patientDetail == null)
            {
                return NotFound();
            }

            return patientDetail;
        }

        // GET: api/Registration/5

        //[HttpGet("{id}", Name = "Get")]
        //public async Task<ActionResult<Patient>> GetPatient(long id)
        //{
        //    var patient = await _context.Patient.FindAsync(id);

        //    if (patient == null)
        //    {
        //        return NotFound();
        //    }

        //    return patient;
        //}

        [Route("RegistrationWithMessage")]
        [HttpPost]
        public async Task<ActionResult<bool>> RegistrationExtended([FromBody] RegistrationDTO dto)
        {
            bool commandResult = false;

            var command = new RegistrationWithMessageCommand(dto.ClientId, dto.FacilityCode, dto.MedicalRecordNumber, dto.PatientNumber, dto.ADTMessage);
            _logger.LogInformation("-----Sending command: RegistrationCommand");

            commandResult = await _mediatr.Send(command);

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();


        }

        // PUT: api/Registration/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
