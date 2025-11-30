using HMSMAL.Common;
using HMSMAL.DTO.Masters;

namespace HMSBAL.Interfaces.Masters
{
    /// <summary>
    /// Service interface for appointments
    /// </summary>
    public interface IAppointmentsService
    {
        /// <summary>
        /// Retrives appointments data based on patientId and doctorId
        /// If patientId and/or doctorId not passed retrives all data 
        /// Else retrives data for the given patientId and/or doctorId
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="doctorId">Doctor Id</param>
        /// <returns></returns>
        public Response GetAppointments(int patientId, int doctorId);

        /// <summary>
        /// Retrieves the available doctors data
        /// </summary>
        /// <returns></returns>
        public Response GetDoctors();

        /// <summary>
        /// Retrieves the available patients data
        /// </summary>
        /// <returns></returns>
        public Response GetPatients();

        /// <summary>
        /// Creates a new appointment using the specified appointment details
        /// </summary>
        /// <param name="objDTOAppointment">Instance of DTOAppointment class</param>
        /// <returns></returns>
        public Response BookAppointment(DTOAppointment objDTOAppointment);

        /// <summary>
        /// Update appointment status for given appointmentId
        /// </summary>
        /// <param name="appointmentId">Appointment Id</param>
        /// <param name="status">Status value to be updated</param>
        /// <returns></returns>
        public Response UpdateAppointmentStatus(int appointmentId, int status);

    }
}
