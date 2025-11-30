using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model For Appointments
    /// </summary>
    public class Appointments
    {
        /// <summary>
        /// Appointment Id
        /// </summary>
        public int AppointmentId { get; set; }

        /// <summary>
        /// Patient Id
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Doctor Id
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// Appointment DateTime
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Status of Appointment (1 - Scheduled, 2 - Completed, 3 - Cancelled)
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// Notes for the Appointment
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Appointment Creation DateTime
        /// </summary>
        [IgnoreOnUpdate]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// Appointment Last Modified DateTime
        /// </summary>
        [IgnoreOnInsert]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
}
