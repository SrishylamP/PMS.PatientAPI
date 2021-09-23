using PMS.PatientAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PatientAPI.Services
{
    public interface IPatientService
    {
        Task<ResponseMessage> PatientRegistration(UserModel model);
        Task<ResponseMessage> SavePatientDetails(PatientModel model);
        Task<List<UserModel>> GetPatientDetails(string email, string roleId);        
        Task<ResponseMessage> SavePatientEmergencyDetails(List<PatientEmergencyDetailModel> model);
        Task<List<PatientEmergencyDetailModel>> GetEmergencyPatientDetails(string email,string roleId);
        Task<List<UserModel>> GetAllPatientDetails();
        Task<ResponseMessage> DeletePatientEmergencyDetails(int patientId, int emergencyContactId);
        Task<List<AllergyModel>> GetAllPatientAllergyDetails();
        Task<List<AllergyModel>> GetPatientAllergyDetails(int PatientId);
        Task<ResponseMessage> SavePatientAllergyDetails(AllergyModel model, int PatientId);
        Task<ResponseMessage> DeletePatientAllergyDetails(int patientId, int allergyId);
        Task<List<DiagnosesModel>> GetPatientDiagnosisDetails(int PatientId,int AppointmentId);
        Task<ResponseMessage> SavePatientDiagnosisDetails(DiagnosesModel model, int PatientId,int AppointmentId,int userId);
        Task<ResponseMessage> DeletePatientDiagnosisDetails(int patientId, int AppointmentId,int diagnosisId);

        Task<List<AllergyModel>> GetAllAllergies();
        Task<List<DiagnosesTypeModel>> GetDiagnosesTypes();
        Task<List<DiagnosesModel>> GetAllDiagnoses();
        Task<List<MedicationModel>> GetAllMedications();
        Task<List<ProcedureModel>> GetAllProcedures();

        Task<List<DiagnosesModel>> GetDiagnosisByPatientId(int PatientId);
        Task<List<ProcedureModel>> GetProcedureByPatientId(int PatientId);
        Task<List<MedicationModel>> GetMedicationsByPatientId(int PatientId);
        //Task<ResponseMessage> SavePatientDiagnosisDetails(DiagnosesModel model, int PatientId, int AppointmentId, int userId);
        //Task<ResponseMessage> DeletePatientDiagnosisDetails(int patientId, int AppointmentId, int diagnosisId);
        
    }

   
}
