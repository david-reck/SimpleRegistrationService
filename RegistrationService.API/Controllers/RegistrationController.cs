﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Data;
using RegistrationService.Data.Queries;

namespace RegistrationService.API.Controllers
{
    [Route("api/registrations")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private RegistrationContext _context;
        private IRegistrationQueries _registrationQueries;

        public RegistrationController(RegistrationContext context, IRegistrationQueries registrationQueries)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _registrationQueries = registrationQueries ?? throw new ArgumentNullException(nameof(registrationQueries));
        }
        // GET: api/Registration
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patient.ToListAsync();
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

        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Patient>> GetPatient(long id)
        {
            var patient = await _context.Patient.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        //POST: api/Registration
       [HttpPost]
        public async Task<ActionResult<bool>> Registration([FromBody] PatientDTO dto)
        {
            var command = new RegistrationCommand(dto.BirthDate, dto.Gender, dto.FirstName, dto.MiddleName, dto.LastName);
            var handler = new RegistrationCommandHandler(_context);

            return await handler.Handle(command, new System.Threading.CancellationToken());


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
