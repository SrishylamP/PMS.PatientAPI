using System;
using System.Collections.Generic;
using System.Text;
namespace PMS.PatientAPI.Models
{
    public class VitalModel
    {

        public int VitalId { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BloodPressure { get; set; }
        public string BodyTemprature { get; set; }
        public string RespirationRate { get; set; }

    }
}

