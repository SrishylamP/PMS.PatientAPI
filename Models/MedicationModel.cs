using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PatientAPI.Models
{
   public class MedicationModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string DrugForm { get; set; }
        public string DrugStrength { get; set; }
        public string DrugBrandName { get; set; }
        public string ReferenceStandard { get; set; }
    }
}
