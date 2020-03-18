using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationService.Data.Queries
{
    public class RegistrationQueries : IRegistrationQueries
    {
        private string _connectionString = string.Empty;
        public RegistrationQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }
        public  async Task<PatientDetail> GetPatientByAccountNumAsync(Int64 patientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<dynamic>(
                   @"SELECT PatientId, BirthDate, Gender, FirstName, MiddleName, LastName
                    FROM dbo.Patient
                    WHERE PatientId = @patientId"
                        , new { patientId }
                    );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return MapPatientDetail(result);
            }
        }

        public async Task<IEnumerable<RegistrationSummary>> GetRegistrationsModifiedAfterAsync(DateTime modifiedAfter)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<RegistrationSummary>(
                   @"SELECT PatientId, BirthDate, Gender, FirstName, MiddleName, LastName, LastUpdateDate, CreateDate, StreetAddress, StreetAddress2, City, State, ZipCode
                        FROM vw_RegistrationSummary
                        WHERE LastUpdateDate > @modifiedAfter
                        ORDER BY LastUpdateDate DESC"
                        , new { modifiedAfter }
                    );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return result;
            }
            
            

        }

        private PatientDetail MapPatientDetail(dynamic result)
        {
            var patientDetail = new PatientDetail
            {
                PatientId = result[0].PatientId,
                BirthDate = result[0].BirthDate,
                Gender = result[0].Gender,
                FirstName = result[0].FirstName,
                MiddleName = result[0].MiddleName,
                LastName = result[0].LastName
            };

            return patientDetail;
        }
    }
}
