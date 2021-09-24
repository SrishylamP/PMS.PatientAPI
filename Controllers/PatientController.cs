using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using PMS.PatientAPI.Services;
using PMS.PatientAPI.Models;

namespace PMS.PatientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            this.logger = logger;
        }
        [HttpPost]
        [Route("PatientRegistration")]
        public async Task<IActionResult> PatientRegistration([FromBody] UserModel model)
        {
            try
            {
                var result = await _patientService.PatientRegistration(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }

        [HttpPost]
        [Authorize]
        [Route("SavePatientDetails")]
        public async Task<IActionResult> SavePatientDetails([FromBody] PatientModel model)
        {
            try
            {
                var result = await _patientService.SavePatientDetails(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet]
        [Route("GetPatientDetails")]
        public async Task<IActionResult> GetPatientDetails(string email, string roleId)
        {
            try
            {
                var result = await _patientService.GetPatientDetails(email, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpPost]
        [Route("SavePatientEmergencyDetails")]
        public async Task<IActionResult> SavePatientEmergencyDetails([FromBody] List<PatientEmergencyDetailModel> model)
        {
            try
            {
                var result = await _patientService.SavePatientEmergencyDetails(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet]
        [Route("GetEmergencyPatientDetails")]
        public async Task<IActionResult> GetEmergencyPatientDetails(string email, string roleId)
        {
            try
            {
                var result = await _patientService.GetEmergencyPatientDetails(email, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpDelete]
        [Route("DeletePatientEmergencyDetails")]
        public async Task<IActionResult> DeletePatientEmergencyDetails(int patientId, int emergencyContactId)
        {
            try
            {
                var result = await _patientService.DeletePatientEmergencyDetails(patientId, emergencyContactId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetAllPatientDetails")]
        public async Task<IActionResult> GetAllPatientDetails()
        {
            try
            {
                var result = await _patientService.GetAllPatientDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet]
        [Route("GetAllPatientAllergyDetails")]
        public async Task<IActionResult> GetAllPatientAllergyDetails()
        {
            try
            {
                var result = await _patientService.GetAllPatientAllergyDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet()]
        [Route("GetPatientAllergyDetails")]
        public async Task<IActionResult> GetPatientAllergyDetails(int PatientId)
        {
            try
            {
                var result = await _patientService.GetPatientAllergyDetails(PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpPost]
        [Authorize]
        [Route("SavePatientAllergyDetails")]
        public async Task<IActionResult> SavePatientAllergyDetails([FromBody] AllergyModel model, int PatientId)
        {
            try
            {
                var result = await _patientService.SavePatientAllergyDetails(model, PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpDelete]
        [Route("DeletePatientAllergyDetails")]
        public async Task<IActionResult> DeletePatientAllergyDetails(int patientId, int allergyId)
        {
            try
            {
                var result = await _patientService.DeletePatientAllergyDetails(patientId, allergyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }

        [Authorize]
        [HttpGet()]
        [Route("GetPatientDiagnosisDetails")]
        public async Task<IActionResult> GetPatientDiagnosisDetails(int PatientId,int AppointmentId)
        {
            try
            {
                var result = await _patientService.GetPatientDiagnosisDetails(PatientId,AppointmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpPost]
        [Authorize]
        [Route("SavePatientDiagnosisDetails")]
        public async Task<IActionResult> SavePatientDiagnosisDetails([FromBody] DiagnosesModel model, int PatientId, int AppointmentId,int userId)
        {
            try
            {
                var result = await _patientService.SavePatientDiagnosisDetails(model, PatientId, AppointmentId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpDelete]
        [Route("DeletePatientDiagnosisDetails")]
        public async Task<IActionResult> DeletePatientDiagnosisDetails(int patientId, int appointmentId, int diagnosisId)
        {
            try
            {
                var result = await _patientService.DeletePatientDiagnosisDetails(patientId, appointmentId, diagnosisId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpGet]
        [Route("GetAllAllergies")]
        [Authorize]
        public async Task<IActionResult> GetAllAllergies()
        {
            try
            {
                var result = await _patientService.GetAllAllergies();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }
        [HttpGet]
        [Route("GetDiagnosesTypes")]
        [Authorize]
        public async Task<IActionResult> GetDiagnosesTypes()
        {
            try
            {
                var result = await _patientService.GetDiagnosesTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }

        [HttpGet]
        [Route("GetAllDiagnoses")]
        [Authorize]
        public async Task<IActionResult> GetAllDiagnoses()
        {
            try
            {
                var result = await _patientService.GetAllDiagnoses();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }

        [HttpGet]
        [Route("GetAllMedications")]
        [Authorize]
        public async Task<IActionResult> GetAllMedications()
        {
            try
            {
                var result = await _patientService.GetAllMedications();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }

        [HttpGet]
        [Route("GetAllProcedures")]
        [Authorize]
        public async Task<IActionResult> GetAllProcedures()
        {
            try
            {
                var result = await _patientService.GetAllProcedures();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }
        [Authorize]
        [HttpGet()]
        [Route("GetDiagnosisByPatientId")]
        public async Task<IActionResult> GetDiagnosisByPatientId(int PatientId)
        {
            try
            {
                var result = await _patientService.GetDiagnosisByPatientId(PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet()]
        [Route("GetProcedureByPatientId")]
        public async Task<IActionResult> GetProcedureByPatientId(int PatientId)
        {
            try
            {
                var result = await _patientService.GetProcedureByPatientId(PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet()]
        [Route("GetMedicationsByPatientId")]
        public async Task<IActionResult> GetMedicationsByPatientId(int PatientId)
        {
            try
            {
                var result = await _patientService.GetMedicationsByPatientId(PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpGet()]
        [Route("GetVitalSignsByPatientId")]
        public async Task<IActionResult> GetVitalSignsByPatientId(int PatientId)
        {
            try
            {
                var result = await _patientService.GetVitalSignsByPatientId(PatientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        [Authorize]
        [HttpPost]
        [Route("UpdatePatientEmergencyDetails")]
        public async Task<IActionResult> UpdatePatientEmergencyDetails([FromBody] List<PatientEmergencyDetailModel> model)
        {
            try
            {
                var result = await _patientService.UpdatePatientEmergencyDetails(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
        


    }
}
