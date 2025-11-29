using HMSMAL.Common;
using HMSMAL.DTO.Masters;

namespace HMSBAL.Interfaces.Masters
{
    /// <summary>
    /// Service interface for appointments
    /// </summary>
    public interface IAppointmentsService
    {
        public Response GetAppointments(int patientId, int doctorId);
        
        public Response GetDoctors();
        
        public Response GetPatients();

        public Response BookAppointment(DTOAppointment objDTOAppointment);

        public Response UpdateAppointmentStatus(int appointmentId, int status);

    }
}
