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

        Task<List<DiagnosesModel>> GetDiagnosisByPatientId(int PatientId, int appointmentId);
        Task<List<ProcedureModel>> GetProcedureByPatientId(int PatientId, int appointmentId);
        Task<List<MedicationModel>> GetMedicationsByPatientId(int PatientId, int appointmentId);
        Task<List<VitalSignsModel>> GetVitalSignsByPatientId(int PatientId, int appointmentId);

        Task<ResponseMessage> UpdatePatientEmergencyDetails(List<PatientEmergencyDetailModel> model);
        

        //Task<ResponseMessage> SavePatientDiagnosisDetails(DiagnosesModel model, int PatientId, int AppointmentId, int userId);
        //Task<ResponseMessage> DeletePatientDiagnosisDetails(int patientId, int AppointmentId, int diagnosisId);
        Task<List<ProcedureModel>> GetPatientProcedureDetails(int PatientId, int AppointmentId);
        Task<ResponseMessage> SavePatientProcedureDetails(ProcedureModel model, int PatientId, int AppointmentId, int userId);
        Task<ResponseMessage> DeletePatientProcedureDetails(int patientId, int AppointmentId, int ProcedureId);
        Task<List<MedicationModel>> GetPatientMedicationDetails(int PatientId, int AppointmentId);
        Task<ResponseMessage> SavePatientMedicationDetails(MedicationModel model, int PatientId, int AppointmentId, int userId);
        Task<ResponseMessage> DeletePatientMedicationDetails(int patientId, int AppointmentId, int DrugId);
        Task<ResponseMessage> ClosePatientVisit(int PatientId, int AppointmentId);
        Task<List<UserModel>> GetPatientDetailsForVisit(int patientId);
        Task<ResponseMessage> SavePatientVitalDetails(VitalModel model);
        Task<List<VitalModel>> GetPatientVitalDetails(int patientId);
    }

   
}
