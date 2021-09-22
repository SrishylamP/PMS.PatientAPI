using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PatientAPI.Models
{
   public class ProcedureModel
    {
        public int ProcedureId { get; set; }
        public string ProcedureCode { get; set; }
        public string ProcedureName { get; set; }
        public string ProcedureApproach { get; set; }
        public bool ProcedureIsDepricated { get; set; }
    }
}
