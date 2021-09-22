using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PatientAPI.Models
{
  public class AllergyModel
    {
        public int AllergyId { get; set; }
        public string AllergyCode { get; set; }
        public string AllergyType { get; set; }
        public string AllergyName { get; set; }
        public string AllergyDescription { get; set; }
        public string AllergyClinicalInformation { get; set; }
        public string Allerginicity { get; set; }
        public bool? IsAllergyFatal { get; set; }
    }
}
