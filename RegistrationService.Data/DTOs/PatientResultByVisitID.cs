using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Data.DTOs
{
    public class PatientResultByVisitID
    {
        //Patient Info



        public string PatientName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIPCode { get; set; }
        public string Country { get; set; }
        public string Race { get; set; }
        public string Religion { get; set; }



        // Diagnostic Info



        public string DiagnosisDescription { get; set; }
        public string DiagnosisType { get; set; }
        public string AdmissionType { get; set; }
        public string AdmitSource { get; set; }
        public string AdmittingDoctor { get; set; }
        public string AttendingDoctor { get; set; }
        public string PatientClass { get; set; }
        public string PatientType { get; set; }
        public string PatientStatusCode { get; set; }



        // Primary Insurance Details



        public string PrimInsurPlanName { get; set; }
        public string PrimInsurAddress { get; set; }
        public string PrimInsurCity { get; set; }
        public string PrimInsurState { get; set; }
        public string PrimInsurZIPCode { get; set; }
        public string PrimPolicyNumber { get; set; }
        public string PrimInsuredDob { get; set; }
        public string PrimInsuredName { get; set; }
        public string PrimInsuredRelationshipToPatient { get; set; }
        public string PrimInsuredSex { get; set; }



        // Sec Insurance Details



        public string SecInsurAddress { get; set; }
        public string SecInsurCity { get; set; }
        public string SecInsurState { get; set; }
        public string SecInsurZIPCode { get; set; }
        public string SecPolicyNumber { get; set; }
        public string SecPhoneNumber { get; set; }
        public string SecInsuredName { get; set; }
        public string SecInsuredRelationshipToPatient { get; set; }
        public string SecInsuredSex { get; set; }
    }
}
