using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PatientAPI.Models
{
   public class DiagnosesModel
    {
        public int DiagnosisId { get; set; }
        public string DiagnosisCode { get; set; }
        public string DiagnosisName { get; set; }
        public int DiagnosisTypeId { get; set; }
        public bool DiagnosisIsDepricated { get; set; }
    }
}
