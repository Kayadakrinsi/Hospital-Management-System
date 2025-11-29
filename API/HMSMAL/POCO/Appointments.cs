using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model For Appointments
    /// </summary>
    public class Appointments
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int Status { get; set; } = 1;
        public string Notes { get; set; } = string.Empty;

        [IgnoreOnUpdate]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [IgnoreOnInsert]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
}
