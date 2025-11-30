using HMSBAL.Common;
using HMSBAL.Interfaces.Masters;
using HMSDAL.Common;
using HMSMAL.Common;
using HMSMAL.DTO.Masters;
using HMSMAL.POCO;
using NLog;
using ServiceStack.OrmLite;

namespace HMSBAL.Services.Masters
{
    /// <summary>
    /// Handler class for user rights management
    /// </summary>
    public class BLAppointmentsHandler : IAppointmentsService
    {
        #region Private Members

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response _response;

        /// <summary>
        /// Stores instance of Logger class
        /// </summary>
        Logger _logger;

        /// <summary>
        /// Stores instance of Appointments class
        /// </summary>
        Appointments pocoAppointments;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary sercvices/objects
        /// </summary>
        public BLAppointmentsHandler()
        {
            _response = new();
            _logger = LogManager.GetLogger("appLog");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrives appointments data based on patientId and doctorId
        /// If patientId and/or doctorId not passed retrives all data 
        /// Else retrives data for the given patientId and/or doctorId
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="doctorId">Doctor Id</param>
        /// <returns></returns>
        public Response GetAppointments(int patientId, int doctorId)
        {
            List<Appointments> lstAppointments = new();

            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    var query = db.From<Appointments>();

                    if (patientId > 0)
                        query.Where(x => x.PatientId == patientId);

                    if (doctorId > 0)
                        query.Where(x => x.DoctorId == doctorId);

                    query.OrderByDescending(x => x.AppointmentId);

                    lstAppointments = db.Select(query);
                }

                if (lstAppointments.Count > 0)
                {
                    _response.Data = lstAppointments;
                }
                else
                {
                    _response.ErrorCode = EnmErrorCodes.E0003;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetAppointments");
                _response.ErrorCode = EnmErrorCodes.E0003;
                throw ex;
            }

            return _response;
        }

        /// <summary>
        /// Retrieves the available doctors data
        /// </summary>
        /// <returns></returns>
        public Response GetDoctors()
        {
            List<Users> lstDoctors = new();
            List<Departments> lstDepartments = new();

            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    lstDoctors = db.Select<Users>(x => x.RoleId == 2 && x.IsActive == 1);
                    lstDepartments = db.Select<Departments>();
                }

                if (lstDoctors.Count > 0)
                {
                    _response.Data = lstDoctors.Select(u => new
                    {
                        u.UserId,
                        u.UserName,
                        u.Email,
                        u.Contact,
                        u.Gender,
                        u.CreatedOn,
                        u.DepartmentId,
                        u.RoleId,
                        Department = lstDepartments.FirstOrDefault(d => d.DepartmentId == u.DepartmentId)?.DepartmentName
                    }).ToList(); ;
                }
                else
                {
                    _response.ErrorCode = EnmErrorCodes.E0003;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetPatients");
                _response.ErrorCode = EnmErrorCodes.E0003;
                throw ex;
            }

            return _response;
        }

        /// <summary>
        /// Retrieves the available patients data
        /// </summary>
        /// <returns></returns>
        public Response GetPatients()
        {
            List<Users> lstPatients = new();

            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    lstPatients = db.Select<Users>(x => x.RoleId == 3 && x.IsActive == 1);
                }

                if (lstPatients.Count > 0)
                {
                    _response.Data = lstPatients;
                }
                else
                {
                    _response.ErrorCode = EnmErrorCodes.E0003;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetPatients");
                _response.ErrorCode = EnmErrorCodes.E0003;
                throw ex;
            }

            return _response;
        }

        #region Book Appointment

        /// <summary>
        /// Creates a new appointment using the specified appointment details
        /// </summary>
        /// <param name="objDTOAppointment">Instance of DTOAppointment class</param>
        /// <returns></returns>
        public Response BookAppointment(DTOAppointment objDTOAppointment)
        {
            _response = ValidateAppointmentBooking(objDTOAppointment);

            if (!_response.IsError)
            {
                PreSaveAppointmentBooking(objDTOAppointment);
                _response = SaveAppointmentBooking();
            }

            return _response;
        }

        /// <summary>
        /// Validates the appointment booking details
        /// </summary>
        /// <param name="objDTOAppointment">Instance of DTOAppointment class</param>
        /// <returns></returns>
        public Response ValidateAppointmentBooking(DTOAppointment objDTOAppointment)
        {
            bool isTaken = false;
            using (var db = new MySqlOrmLite().Open())
            {
                // Check if doctor already has appointment at that time
                isTaken = db.Exists<Appointments>(x =>
                   x.DoctorId == objDTOAppointment.DoctorId &&
                   x.AppointmentDate == objDTOAppointment.AppointmentDate &&
                   x.Status != 3
               );
            }

            if (isTaken)
            {
                _response.ErrorCode = EnmErrorCodes.E0004;
            }

            return _response;
        }

        /// <summary>
        /// Prepares the appointment booking details to be saved
        /// </summary>
        /// <param name="objDTOAppointment">Instance of DTOAppointment class</param>
        public void PreSaveAppointmentBooking(DTOAppointment objDTOAppointment)
        {
            pocoAppointments = new();
            BLGlobalClass.CopyProperties<DTOAppointment, Appointments>(objDTOAppointment, pocoAppointments);
        }

        /// <summary>
        /// Saves the appointment booking details
        /// </summary>
        /// <returns></returns>
        public Response SaveAppointmentBooking()
        {
            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    db.Insert(pocoAppointments);
                }
                _response.Message = SuccessMessages.S0005;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in SaveAppointmentBooking");
                _response.ErrorCode = EnmErrorCodes.E0001;
                throw ex;
            }

            return _response;
        }

        #endregion

        /// <summary>
        /// Update appointment status for given appointmentId
        /// </summary>
        /// <param name="appointmentId">Appointment Id</param>
        /// <param name="status">Status value to be updated</param>
        /// <returns></returns>
        public Response UpdateAppointmentStatus(int appointmentId, int status)
        {
            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    pocoAppointments = db.SingleById<Appointments>(appointmentId);

                    if (pocoAppointments == null)
                    {
                        _response.ErrorCode = EnmErrorCodes.A0001;
                        return _response;
                    }

                    db.UpdateOnlyFields<Appointments>(new Appointments { Status = status },
                        where: _appointment => _appointment.AppointmentId == pocoAppointments.AppointmentId,
                        onlyFields: _appointment => new { _appointment.Status });
                }

                _response.Message = SuccessMessages.S0006;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in UpdateAppointmentStatus");
                _response.ErrorCode = EnmErrorCodes.A0001;
                throw ex;
            }

            return _response;
        }

        #endregion

    }
}
