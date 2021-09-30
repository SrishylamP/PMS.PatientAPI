using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PMS.PatientAPI.Data;
using PMS.PatientAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PatientAPI.Services
{
    public class PatientDbService : IPatientService
    {
        private readonly PMSDbContext _context;

        public PatientDbService(PMSDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseMessage> PatientRegistration(UserModel model)
        {
            try
            {
                var resObj = new ResponseMessage();
                var objEmail = _context.Users.FirstOrDefault((e) => e.Email == model.Email);

                if (objEmail != null)
                {
                    resObj.message = "Email Exist";
                    resObj.IsSuccess = false;
                }
                else
                {
                    User user = new User();
                    user.Title = model.Title;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Dob = model.DOB.ToLocalTime();
                    user.EmployeeId = model.EmployeeId;
                    user.Password = Common.CreateMD5(model.Password);
                    user.NoOfWrongAttempts = model.NoOfWrongAttempts;
                    user.IsActive = true;
                    user.IsFirstTimeUser = false;
                    user.RoleId = 4;
                    user.Gender = model.Gender;
                    user.CreatedDate = DateTime.Now;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    SavePatientRegistrationDetails(model);
                    resObj.IsSuccess = true;
                    resObj.message = "Registration successful";

                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient",
                        Description = $"Patient Created : {user.Email}",
                        CreatedBy = user.UserId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string SavePatientRegistrationDetails(UserModel model)
        {

            var objuserId = _context.Users.FirstOrDefault((e) => e.Email == model.Email);
            PatientDetail patientDetail = new PatientDetail();
            patientDetail.UserId = objuserId.UserId;
            patientDetail.Age = get_age(Convert.ToDateTime(model.DOB.ToLocalTime()));
            _context.PatientDetails.Add(patientDetail);
            _context.SaveChanges();
            return "Success";

        }

        public async Task<ResponseMessage> SavePatientDetails(PatientModel model)
        {
            try
            {
                var resObj = new ResponseMessage();
                var entity = await _context.PatientDetails.FirstOrDefaultAsync(item => item.UserId == model.UserId);

                if (entity != null)
                {

                    entity.Race = model.Race;
                    entity.Ethnicity = model.Ethnicity;
                    entity.HomeAddress = model.HomeAddress;
                    entity.LanguagesKnow = model.LanguagesKnown;
                    _context.PatientDetails.Update(entity);
                    _context.SaveChanges();

                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Patient Details",
                        Description = $"Patient Details updated Id: {entity.PatientId}",
                        CreatedBy = entity.PatientId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Patient details save successfully";
                }

                else
                {
                    resObj.message = "Patient detail is invalid";
                }

                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<List<UserModel>> GetPatientDetails(string email, string roleId)
        {

            var objectList = await (from u in _context.Users
                                    where u.Email == email && u.RoleId == Convert.ToInt32(roleId)
                                    select u).Select
                (o => new UserModel
                {
                    Title = o.Title,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    DOB = o.Dob,
                    Gender = o.Gender,
                    Email = o.Email,
                    UserId = o.UserId,
                    Phone = o.Phone,
                    Patient = o.PatientDetails.Select(pd => new PatientModel
                    {
                        Age = pd.Age,
                        Race = pd.Race,
                        Ethnicity = pd.Ethnicity,
                        HomeAddress = pd.HomeAddress,
                        LanguagesKnown = pd.LanguagesKnow,
                        PatientId = pd.PatientId,
                        UserId = pd.UserId,
                    }).ToList()
                }).ToListAsync();

            return objectList;

            // return list;
        }


        public int get_age(DateTime dob)
        {
            int age = 0;
            age = DateTime.Now.Subtract(dob).Days;
            age = age / 365;
            return age;
        }

        public async Task<ResponseMessage> SavePatientEmergencyDetails(List<PatientEmergencyDetailModel> model)
        {
            try
            {
                EmergencyContactDetail contactDetails = new EmergencyContactDetail();
                var resObj = new ResponseMessage();
                foreach (var item in model)
                {
                    var patientDeatils = _context.PatientDetails.FirstOrDefault((e) => e.PatientId == item.PatientId);
                    // var emergencyDetails = _context.EmergencyContactDetails.FirstOrDefault(e => e.PatientId == item.PatientId && e.EmergencyContactId == item.EmergencyContactId);
                    if (patientDeatils != null)
                    {

                        contactDetails.PatientId = item.PatientId;
                        contactDetails.Access = item.Access;
                        contactDetails.Address = item.Address;
                        contactDetails.EfisrtName = item.EFirstName;
                        contactDetails.ElastName = item.ELastName;
                        contactDetails.Email = item.Email;
                        contactDetails.Phone = item.Phone;
                        contactDetails.RelationShip = item.RelationShip;
                        contactDetails.CreatedDate = DateTime.Now;
                        await _context.EmergencyContactDetails.AddAsync(contactDetails);
                        await _context.SaveChangesAsync();
                        var audit = new AuditModel
                        {
                            Operation = Constants.Create,
                            ObjectName = "Patient Contact Details",
                            Description = $"Patient Contact Details Created with Id: {contactDetails.EmergencyContactId}",
                            CreatedBy = item.PatientId,
                            CreatedDate = DateTime.Now
                        };
                        AuditMe(audit);
                        resObj.IsSuccess = true;
                        resObj.message = "Successful";
                    }

                    else
                    {
                        resObj.IsSuccess = false;
                        resObj.message = "Patient detail is invalid";
                    }
                }


                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<PatientEmergencyDetailModel>> GetEmergencyPatientDetails(string email, string roleId)
        {
            try
            {
                var list = await (from u in _context.Users
                                  join pd in _context.PatientDetails
                                  on u.UserId equals pd.UserId
                                  join ecd in _context.EmergencyContactDetails
                                  on pd.PatientId equals ecd.PatientId
                                  where u.Email == email && u.RoleId == Convert.ToInt32(roleId)

                                  // where ecd.PatientId == patientId

                                  select new PatientEmergencyDetailModel
                                  {
                                      EFirstName = ecd.EfisrtName,
                                      ELastName = ecd.ElastName,
                                      Email = ecd.Email,
                                      EmergencyContactId = ecd.EmergencyContactId,
                                      Access = ecd.Access,
                                      Address = ecd.Address,
                                      Phone = ecd.Phone,
                                      RelationShip = ecd.RelationShip,
                                      PatientId = ecd.PatientId,
                                  }).ToListAsync();
                return list;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseMessage> DeletePatientEmergencyDetails(int patientId, int emergencyContactId)
        {
            ResponseMessage response = new ResponseMessage();
            EmergencyContactDetail emergencyContactDetails;
            try
            {

                var db = await _context.EmergencyContactDetails.FirstOrDefaultAsync(x => x.PatientId == patientId && x.EmergencyContactId == emergencyContactId);
                if (db != null)
                {
                    emergencyContactDetails = _context.EmergencyContactDetails.Where(x => x.PatientId == patientId && x.EmergencyContactId == emergencyContactId).FirstOrDefault();

                    _context.EmergencyContactDetails.Remove(emergencyContactDetails);
                    _context.SaveChanges();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Delete,
                        ObjectName = "Patient Contact Details",
                        Description = $"Patient Contact Details Deleted Id: {emergencyContactDetails.EmergencyContactId}",
                        CreatedBy = emergencyContactDetails.PatientId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    response.IsSuccess = true;
                    response.message = "Deleted Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.message = "Invalid  Details";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<List<UserModel>> GetAllPatientDetails()
        {

            var list = await (from u in _context.Users
                              join r in _context.UserRoles
                              on u.RoleId equals r.RoleId
                              where u.RoleId == 4
                              select new UserModel
                              {
                                  Title = u.Title,
                                  UserId = u.UserId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Phone = u.Phone,
                                  DOB = u.Dob,
                                  EmployeeId = u.EmployeeId,
                                  NoOfWrongAttempts = u.NoOfWrongAttempts,
                                  Role = r.RoleName,
                                  RoleId = r.RoleId,
                                  IsActive = u.IsActive,
                                  IsBlocked = u.IsBlocked,
                                  Status = u.IsBlocked ? "Blocked" : u.IsActive ? "Active" : "InActive",
                                  CreatedDate = u.CreatedDate,
                                  Gender = u.Gender,

                                  Patient = u.PatientDetails.Select(pd => new PatientModel
                                  {
                                      Age = pd.Age,
                                      Race = pd.Race,
                                      Ethnicity = pd.Ethnicity,
                                      HomeAddress = pd.HomeAddress,
                                      LanguagesKnown = pd.LanguagesKnow,
                                      PatientId = pd.PatientId,
                                      UserId = pd.UserId,
                                  }).ToList()
                              }).ToListAsync();
            return list;
        }
        public async Task<List<AllergyModel>> GetAllPatientAllergyDetails()
        {
            var allergyList = await (from al in _context.Allergies

                                     select new AllergyModel
                                     {
                                         AllergyId = al.AllergyId,
                                         AllergyDescription = al.AllergenDescription,
                                         Allerginicity = al.Allerginicity,
                                         AllergyClinicalInformation = al.AllergyClinicalInformation,
                                         AllergyCode = al.AllergyCode,
                                         AllergyName = al.AllergyName,
                                         AllergyType = al.AllergyType
                                     }).ToListAsync();
            return allergyList;
        }
        public async Task<List<AllergyModel>> GetPatientAllergyDetails(int PatientId)
        {
            try
            {
                var allergyList = await (from pa in _context.PatientAllergyDetails
                                         where pa.PatientId == PatientId

                                         select new AllergyModel
                                         {
                                             AllergyDescription = pa.AllergenDescription,
                                             AllergyId = pa.AllergyId,
                                             AllergyClinicalInformation = pa.AllergyClinicalInformation,
                                             AllergyName = pa.AllergyName,
                                             AllergyType = pa.AllergyType,
                                             IsAllergyFatal = pa.IsAllergyFatal
                                         }).ToListAsync();


                return allergyList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseMessage> SavePatientAllergyDetails(AllergyModel model, int PatientId)
        {
            try
            {
                var allergyDetail = _context.PatientAllergyDetails.FirstOrDefault((e) => e.PatientId == PatientId && e.AllergyId == model.AllergyId);
                var resObj = new ResponseMessage();
                if (allergyDetail == null)
                {
                    PatientAllergyDetail pad = new PatientAllergyDetail();
                    pad.PatientId = PatientId;
                    pad.AllergyId = model.AllergyId;
                    pad.AllergyName = model.AllergyName;
                    pad.AllergyType = model.AllergyType;
                    pad.AllergenDescription = model.AllergyDescription;
                    pad.AllergyClinicalInformation = model.AllergyClinicalInformation;
                    pad.IsAllergyFatal = model.IsAllergyFatal;
                    await _context.PatientAllergyDetails.AddAsync(pad);
                    await _context.SaveChangesAsync();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient Contact Details",
                        Description = $"Patient Allergy Details Created with PatientId: {pad.PatientId + "AllergyId" + pad.AllergyId}",
                        CreatedBy = pad.PatientId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Successful";

                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = "Allergy Details Already Exist, Please enter new Allergy Details";
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ResponseMessage> DeletePatientAllergyDetails(int patientId, int alleryId)
        {
            ResponseMessage response = new ResponseMessage();
            PatientAllergyDetail patientAllergyDetail;
            try
            {

                patientAllergyDetail = await _context.PatientAllergyDetails.FirstOrDefaultAsync(x => x.PatientId == patientId && x.AllergyId == alleryId);
                if (patientAllergyDetail != null)
                {
                    _context.PatientAllergyDetails.Remove(patientAllergyDetail);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.message = "Deleted Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.message = "Invalid  Details";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<List<DiagnosesModel>> GetPatientDiagnosisDetails(int PatientId, int AppointmentId)
        {
            try
            {
                var DiagnosisList = await (from pd in _context.PatientDiagnosisDetails
                                           join d in _context.Diagnoses
                                            on pd.DiagnosisId equals d.DiagnosisId
                                           where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                           select new DiagnosesModel
                                           {
                                               DiagnosisId = pd.DiagnosisId,
                                               DiagnosisName = d.DiagnosisName,
                                               DiagnosisCode = d.DiagnosisCode,
                                               DiagnosisIsDepricated = d.DiagnosisIsDepricated,
                                               DiagnosisTypeId = d.DiagnosisTypeId
                                           }).ToListAsync();
                return DiagnosisList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseMessage> SavePatientDiagnosisDetails(DiagnosesModel model, int PatientId, int AppointmentId, int userId)
        {
            try
            {
                var diagnosisDetail = _context.PatientDiagnosisDetails.FirstOrDefault((e) => e.PatientId == PatientId && e.AppointmentId == AppointmentId && e.DiagnosisId == model.DiagnosisId);
                var resObj = new ResponseMessage();
                if (diagnosisDetail == null)
                {
                    PatientDiagnosisDetail pad = new PatientDiagnosisDetail();
                    pad.PatientId = PatientId;
                    pad.AppointmentId = AppointmentId;
                    pad.DiagnosisId = model.DiagnosisId;
                    pad.AddedBy = userId;
                    pad.AddedDate = DateTime.Now;
                    await _context.PatientDiagnosisDetails.AddAsync(pad);
                    await _context.SaveChangesAsync();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient Diagnosis Details",
                        Description = $"Patient Diagnosis Details Created with PatientId: {pad.PatientId + "Diagnosis" + pad.DiagnosisId}",
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Successful";

                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = "Diagnosis Details Already Exist, Please enter new Diagnosis Details";
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ResponseMessage> DeletePatientDiagnosisDetails(int patientId, int AppointmentId, int diagnosisId)
        {
            ResponseMessage response = new ResponseMessage();
            PatientDiagnosisDetail patientDiagnosisDetail;
            try
            {

                patientDiagnosisDetail = await _context.PatientDiagnosisDetails.FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == AppointmentId && x.DiagnosisId == diagnosisId);
                if (patientDiagnosisDetail != null)
                {
                    _context.PatientDiagnosisDetails.Remove(patientDiagnosisDetail);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.message = "Deleted Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.message = "Invalid  Details";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<List<AllergyModel>> GetAllAllergies()
        {
            var list = await _context.Allergies.Select(e => new AllergyModel
            {
                AllergyId = e.AllergyId,
                AllergyName = e.AllergyName,
                AllergyCode = e.AllergyCode,
                AllergyType = e.AllergyType,
                AllergyDescription = e.AllergenDescription,
                AllergyClinicalInformation = e.AllergyClinicalInformation,
                Allerginicity = e.Allerginicity
            }).ToListAsync();
            return list;
        }

        public async Task<List<DiagnosesTypeModel>> GetDiagnosesTypes()
        {
            var list = await _context.DiagnosesTypes.Select(e => new DiagnosesTypeModel { DiagnosesTypeId = e.DiagnosesTypeId, DiagnosesType1 = e.DiagnosesType1 }).ToListAsync();
            return list;
        }
        public async Task<List<DiagnosesModel>> GetAllDiagnoses()
        {
            var list = await _context.Diagnoses.Select(e => new DiagnosesModel
            {
                DiagnosisId = e.DiagnosisId,
                DiagnosisTypeId = e.DiagnosisTypeId,
                DiagnosisCode = e.DiagnosisCode,
                DiagnosisName = e.DiagnosisName,
                DiagnosisIsDepricated = e.DiagnosisIsDepricated
            }).ToListAsync();
            return list;
        }
        public async Task<List<MedicationModel>> GetAllMedications()
        {
            var list = await _context.Medications.Select(e => new MedicationModel
            {
                DrugId = e.DrugId,
                DrugName = string.IsNullOrEmpty(e.DrugStrength) ? e.DrugName : e.DrugName + "-" + e.DrugStrength,
                DrugForm = e.DrugForm,
                DrugBrandName = e.DrugBrandName,
                //DrugStrength = e.DrugStrength,
                ReferenceStandard = e.ReferenceStandard
            }).ToListAsync();
            return list;
        }
        public async Task<List<ProcedureModel>> GetAllProcedures()
        {
            var list = await _context.Procedures.Select(e => new ProcedureModel
            {
                ProcedureId = e.ProcedureId,
                ProcedureName = e.ProcedureName,
                ProcedureCode = e.ProcedureCode,
                ProcedureApproach = e.ProcedureApproach,
                ProcedureIsDepricated = e.ProcedureIsDepricated
            }).ToListAsync();
            return list;
        }
        public void AuditMe(AuditModel model)
        {
            var aObj = new Audit
            {
                Operation = model.Operation,
                Description = model.Description,
                ObjectName = model.ObjectName,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };
            _context.Audits.Add(aObj);
            _context.SaveChanges();
        }
        public async Task<List<DiagnosesModel>> GetDiagnosisByPatientId(int PatientId, int AppointmentId)
        {
            try
            {
                var DiagnosisList = await (from pd in _context.PatientDiagnosisDetails
                                           join d in _context.Diagnoses
                                            on pd.DiagnosisId equals d.DiagnosisId
                                           where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                           select new DiagnosesModel
                                           {
                                               DiagnosisId = pd.DiagnosisId,
                                               DiagnosisName = d.DiagnosisName,
                                               DiagnosisCode = d.DiagnosisCode,
                                               DiagnosisIsDepricated = d.DiagnosisIsDepricated,
                                               DiagnosisTypeId = d.DiagnosisTypeId,
                                               AppointmentVisitDate = pd.AddedDate
                                           }).ToListAsync();
                return DiagnosisList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<ProcedureModel>> GetProcedureByPatientId(int PatientId, int AppointmentId)
        {
            try
            {
                var procedureList = await (from pd in _context.PatientProcedureDetails
                                           join d in _context.Procedures
                                            on pd.PatientProcedureId equals d.ProcedureId
                                           where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                           select new ProcedureModel
                                           {
                                               ProcedureId = pd.PatientProcedureId,
                                               ProcedureName = d.ProcedureName,
                                               ProcedureCode = d.ProcedureCode,
                                               ProcedureIsDepricated = d.ProcedureIsDepricated,
                                               ProcedureApproach = d.ProcedureApproach,
                                               AppointmentVisitDate = pd.AddedDate
                                           }).ToListAsync();
                return procedureList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<MedicationModel>> GetMedicationsByPatientId(int PatientId, int AppointmentId)
        {
            try
            {
                var medicationList = await (from pd in _context.PatientMedicationsDetails
                                            join d in _context.Medications
                                             on pd.PatientMedicationId equals d.DrugId
                                            where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                            select new MedicationModel
                                            {
                                                DrugId = pd.DrugId,
                                                DrugName = d.DrugName,
                                                DrugForm = string.IsNullOrEmpty(d.DrugStrength) ? d.DrugName : d.DrugName + "-" + d.DrugStrength,
                                                DrugBrandName = d.DrugBrandName,
                                                DrugStrength = d.DrugStrength,
                                                ReferenceStandard = d.ReferenceStandard,
                                                AppointmentVisitDate = pd.AddedDate
                                            }).ToListAsync();
                return medicationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<VitalSignsModel>> GetVitalSignsByPatientId(int PatientId, int appointmentId)
        {
            try
            {
                var medicationList = await (from pd in _context.PatientVitalDetails
                                            where pd.PatientId == PatientId && pd.AppointmentId == appointmentId
                                            select new VitalSignsModel
                                            {
                                                BloodPressure = pd.BloodPressure,
                                                BodyTemprature = pd.BodyTemprature,
                                                Height = pd.Height,
                                                Weight = pd.Weight,
                                                RespirationRate = pd.RespirationRate,
                                            }).ToListAsync();
                return medicationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ResponseMessage> UpdatePatientEmergencyDetails(List<PatientEmergencyDetailModel> model)
        {
            try
            {
                //EmergencyContactDetail contactDetails = new EmergencyContactDetail();
                var resObj = new ResponseMessage();
                foreach (var item in model)
                {
                   // var patientDeatils = _context.PatientDetails.FirstOrDefault((e) => e.PatientId == item.PatientId);
                    var emergencyDetails = _context.EmergencyContactDetails.FirstOrDefault(e => e.PatientId == item.PatientId && e.EmergencyContactId == item.EmergencyContactId);
                    if (emergencyDetails != null)
                    {

                        //contactDetails.PatientId = item.PatientId;
                        emergencyDetails.Access = item.Access;
                        emergencyDetails.Address = item.Address;
                        emergencyDetails.EfisrtName = item.EFirstName;
                        emergencyDetails.ElastName = item.ELastName;
                        emergencyDetails.Email = item.Email;
                        emergencyDetails.Phone = item.Phone;
                        emergencyDetails.RelationShip = item.RelationShip;
                        emergencyDetails.CreatedDate = DateTime.Now;
                       // _context.Entry(contactDetails).State = EntityState.Modified;
                        _context.EmergencyContactDetails.Update(emergencyDetails);
                        await _context.SaveChangesAsync();
                        var audit = new AuditModel
                        {
                            Operation = Constants.Update,
                            ObjectName = "Patient Contact Details",
                            Description = $"Patient Contact Details Updated with Id: {emergencyDetails.EmergencyContactId}",
                            CreatedBy = item.PatientId,
                            CreatedDate = DateTime.Now
                        };
                        AuditMe(audit);
                        resObj.IsSuccess = true;
                        resObj.message = "Successful Updated";
                    }

                    else
                    {
                        resObj.IsSuccess = false;
                        resObj.message = "Patient detail is invalid";
                    }
                }


                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<ProcedureModel>> GetPatientProcedureDetails(int PatientId, int AppointmentId)
        {
            try
            {
                var ProcedureList = await (from pd in _context.PatientProcedureDetails
                                           join d in _context.Procedures
                                            on pd.ProcedureId equals d.ProcedureId
                                           where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                           select new ProcedureModel
                                           {
                                               ProcedureId = pd.ProcedureId,
                                               ProcedureName = d.ProcedureName,
                                               ProcedureCode = d.ProcedureCode,
                                               ProcedureIsDepricated = d.ProcedureIsDepricated,
                                               ProcedureApproach = d.ProcedureApproach
                                           }).ToListAsync();
                return ProcedureList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseMessage> SavePatientProcedureDetails(ProcedureModel model, int PatientId, int AppointmentId, int userId)
        {
            try
            {
                var procedureDetail = _context.PatientProcedureDetails.FirstOrDefault((e) => e.PatientId == PatientId && e.AppointmentId == AppointmentId && e.ProcedureId == model.ProcedureId);
                var resObj = new ResponseMessage();
                if (procedureDetail == null)
                {
                    PatientProcedureDetail pad = new PatientProcedureDetail();
                    pad.PatientId = PatientId;
                    pad.AppointmentId = AppointmentId;
                    pad.ProcedureId = model.ProcedureId;
                    pad.AddedBy = userId;
                    pad.AddedDate = DateTime.Now;
                    await _context.PatientProcedureDetails.AddAsync(pad);
                    await _context.SaveChangesAsync();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient Procedure Details",
                        Description = $"Patient Procedure Details Created with PatientId: {pad.PatientId + "Procedure" + pad.ProcedureId}",
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Successful";

                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = "Procedure Details Already Exist, Please enter new Procedure Details";
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ResponseMessage> DeletePatientProcedureDetails(int patientId, int AppointmentId, int ProcedureId)
        {
            ResponseMessage response = new ResponseMessage();
            PatientProcedureDetail patientProcedureDetail;
            try
            {

                patientProcedureDetail = await _context.PatientProcedureDetails.FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == AppointmentId && x.ProcedureId == ProcedureId);
                if (patientProcedureDetail != null)
                {
                    _context.PatientProcedureDetails.Remove(patientProcedureDetail);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.message = "Deleted Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.message = "Invalid  Details";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<List<MedicationModel>> GetPatientMedicationDetails(int PatientId, int AppointmentId)
        {
            try
            {
                var MedicationList = await (from pd in _context.PatientMedicationsDetails
                                            join d in _context.Medications
                                             on pd.DrugId equals d.DrugId
                                            where pd.PatientId == PatientId && pd.AppointmentId == AppointmentId
                                            select new MedicationModel
                                            {
                                                DrugId = pd.DrugId,
                                                DrugName = d.DrugName + "-" + d.DrugStrength,
                                                DrugBrandName = d.DrugBrandName,
                                                DrugForm = d.DrugForm,
                                            }).ToListAsync();
                return MedicationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseMessage> SavePatientMedicationDetails(MedicationModel model, int PatientId, int AppointmentId, int userId)
        {
            try
            {
                var medicationDetail = _context.PatientMedicationsDetails.FirstOrDefault((e) => e.PatientId == PatientId && e.AppointmentId == AppointmentId && e.DrugId == model.DrugId);
                var resObj = new ResponseMessage();
                if (medicationDetail == null)
                {
                    PatientMedicationsDetail pmd = new PatientMedicationsDetail();
                    pmd.PatientId = PatientId;
                    pmd.AppointmentId = AppointmentId;
                    pmd.DrugId = model.DrugId;
                    pmd.AddedBy = userId;
                    pmd.AddedDate = DateTime.Now;
                    await _context.PatientMedicationsDetails.AddAsync(pmd);
                    await _context.SaveChangesAsync();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient Medication Details",
                        Description = $"Patient Medication Details Created with PatientId: {pmd.PatientId + "Medication" + pmd.DrugId}",
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Successful";
                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = "Medication Details Already Exist, Please enter new Medicatoin Details";
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ResponseMessage> DeletePatientMedicationDetails(int patientId, int AppointmentId, int DrugId)
        {
            ResponseMessage response = new ResponseMessage();
            PatientMedicationsDetail patientMedicationsDetail;
            try
            {

                patientMedicationsDetail = await _context.PatientMedicationsDetails.FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == AppointmentId && x.DrugId == DrugId);
                if (patientMedicationsDetail != null)
                {
                    _context.PatientMedicationsDetails.Remove(patientMedicationsDetail);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.message = "Deleted Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.message = "Invalid  Details";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<ResponseMessage> ClosePatientVisit(int PatientId, int AppointmentId)
        {
            try
            {
                var appointment = _context.Appointments.FirstOrDefault((a) => a.PatientId == PatientId && a.AppointmentId == AppointmentId);
                var resObj = new ResponseMessage();
                if (appointment != null)
                {
                    appointment.AppointmentStatus = "Closed";
                    await _context.SaveChangesAsync();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Patient Visit Closed",
                        Description = $"Patient Visit Closed with PatientId: {PatientId + "Appointment" + AppointmentId}",
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Successful";

                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = "Appointment Not Exist";
                }
                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<UserModel>> GetPatientDetailsForVisit(int patientId)
        {

            var objectList = await (from u in _context.Users
                                    where u.UserId == patientId
                                    select u).Select
           (o => new UserModel
           {
               Title = o.Title,
               FirstName = o.FirstName,
               LastName = o.LastName,
               DOB = o.Dob,
               Gender = o.Gender,
               Email = o.Email,
               UserId = o.UserId,
               Phone = o.Phone,
               Patient = o.PatientDetails.Select(pd => new PatientModel
               {
                   Age = pd.Age,
                   Race = pd.Race,
                   Ethnicity = pd.Ethnicity,
                   HomeAddress = pd.HomeAddress,
                   LanguagesKnown = pd.LanguagesKnow,
                   PatientId = pd.PatientId,
                   UserId = pd.UserId,
               }).ToList()
           }).ToListAsync();

            return objectList;

            // return list;
        }
        public async Task<ResponseMessage> SavePatientVitalDetails(VitalModel model)
        {
            try
            {
                var resObj = new ResponseMessage();
                var entity = await _context.PatientVitalDetails.FirstOrDefaultAsync(item => item.AppointmentId == model.AppointmentId && item.PatientId == model.PatientId);
                var appentity =await _context.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == model.AppointmentId);

                PatientVitalDetail vital = new PatientVitalDetail();
                if (entity == null)
                {
                    vital.PatientId = model.PatientId;
                    vital.AppointmentId = model.AppointmentId;
                    vital.Height = model.Height;
                    vital.Weight = model.Weight;
                    vital.BloodPressure = model.BloodPressure;
                    vital.BodyTemprature = model.BodyTemprature;
                    vital.RespirationRate = model.RespirationRate;
                    _context.PatientVitalDetails.Add(vital);
                    _context.SaveChanges();
                    if (appentity != null)
                    {
                        appentity.AppointmentStatus = "In Progress";
                        _context.Appointments.Update(appentity);
                    }
                    _context.SaveChanges();
                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Patient Vital Details",
                        Description = $"Patient Vital Signs Saved For Id: {model.PatientId}",
                        CreatedBy = model.PatientId,
                        CreatedDate = DateTime.Now
                    };
                    resObj.IsSuccess = true;
                    resObj.message = "Patient vital details save successfully";
                }
                else if(entity!=null)
                {
                    entity.Height = model.Height;
                    entity.Weight = model.Weight;
                    entity.BloodPressure = model.BloodPressure;
                    entity.BodyTemprature = model.BodyTemprature;
                    entity.RespirationRate = model.RespirationRate;
                    _context.PatientVitalDetails.Update(entity);                
                    _context.SaveChanges();
                    
                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Patient Vital Details",
                        Description = $"Patient Vital Signs updated Id: {entity.PatientId}",
                        CreatedBy = entity.PatientId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Patient vital details Updated successfully";
                }
                else
                {
                    resObj.message = "Patient vital detail is invalid";
                }

                return resObj;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<VitalModel>> GetPatientVitalDetails(int patientId,int AppointmentId)
        {

            var objectList = await (from u in _context.PatientVitalDetails
                                    where u.PatientId == patientId && u.AppointmentId==AppointmentId
                                    select u).Select
           (o => new VitalModel
           {
               VitalId = o.VitalId,
               Height = o.Height,
               Weight = o.Weight,
               BloodPressure = o.BloodPressure,
               BodyTemprature = o.BodyTemprature,
               RespirationRate = o.RespirationRate,
           }).ToListAsync();

            return objectList;

            // return list;
        }

        public async Task<List<AllergyModel>> GetPatientAllergyDetailsByUserId(int patientUserId)
        {

            var allergyList = await (from u in _context.Users
                                     join pd in _context.PatientDetails
                                     on u.UserId equals pd.UserId
                                     join pad in _context.PatientAllergyDetails
                                     on pd.PatientId equals pad.PatientId
                                    where u.UserId == patientUserId
                                     select pad).Select
           (allergy => new AllergyModel
           {
               AllergyId = allergy.AllergyId,
               AllergyName = allergy.AllergyName,
               AllergyType = allergy.AllergyType,
               AllergyDescription = allergy.AllergenDescription,
               AllergyClinicalInformation = allergy.AllergyClinicalInformation,
               IsAllergyFatal = allergy.IsAllergyFatal
           }).ToListAsync();

            return allergyList;

           
        }

    }
}
