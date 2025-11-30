namespace HMSMAL.DTO.Masters
{
    /// <summary>
    /// DTO Model For Appointments
    /// </summary>
    public class DTOAppointment
    {
        /// <summary>
        /// Patient Id
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Doctor Id
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// Appointmnet DateTime
        /// </summary>
        public DateTime AppointmentDate { get; set; }
    }

}
