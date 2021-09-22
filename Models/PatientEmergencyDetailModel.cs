using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PatientAPI.Models
{
    public class PatientEmergencyDetailModel
    {
        public int PatientId { get; set; }
        public int EmergencyContactId { get; set; }
        public string EFirstName { get; set; }
        public string ELastName { get; set; }
        public string RelationShip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Boolean? Access { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
