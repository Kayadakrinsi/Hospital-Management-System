using HMSBAL.Interfaces.Masters;
using HMSMAL.Common;
using HMSMAL.DTO.Masters;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.Masters
{
    /// <summary>
    /// Controller for handling appointments related operations 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CLAppointmentsController : ControllerBase
    {
        #region Private Members

        /// <summary>
        /// Stores instance of IAppointmentsService interface
        /// </summary>
        IAppointmentsService _appoitmentService { get; set; }

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response _response;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary services/objects
        /// </summary>
        /// <param name="appoitmentService">Instance of IAppointmentsService</param>

        public CLAppointmentsController(IAppointmentsService appoitmentService)
        {
            _appoitmentService = appoitmentService;
            _response = new();

        }

        #endregion

        /// <summary>
        /// Retrives appointments data based on patientId and doctorId
        /// If patientId and/or doctorId not passed retrives all data 
        /// Else retrives data for the given patientId and/or doctorId
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="doctorId">Doctor Id</param>
        /// <returns></returns>
        [HttpGet("GetAppointments")]
        public IActionResult GetAppointments(int patientId = 0, int doctorId = 0)
        {
            _response = _appoitmentService.GetAppointments(patientId, doctorId);
            return Ok(_response);
        }

        /// <summary>
        /// Retrieves the available doctors data
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDoctors")]
        public IActionResult GetDoctors()
        {
            _response = _appoitmentService.GetDoctors();
            return Ok(_response);
        }

        /// <summary>
        /// Retrieves the available patients data
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPatients")]
        public IActionResult GetPatients()
        {
            _response = _appoitmentService.GetPatients();
            return Ok(_response);
        }

        /// <summary>
        /// Creates a new appointment using the specified appointment details
        /// </summary>
        /// <param name="objDTOAppointment">Instance of DTOAppointment class</param>
        /// <returns></returns>
        [HttpPost("BookAppointment")]
        public IActionResult BookAppointment(DTOAppointment objDTOAppointment)
        {
            _response = _appoitmentService.BookAppointment(objDTOAppointment);
            return Ok(_response);
        }

        /// <summary>
        /// Completes the appointment having given appointment id
        /// </summary>
        /// <param name="appointmentId">Appointment Id</param>
        /// <returns></returns>
        [HttpPut("CompleteAppointment")]
        public IActionResult CompleteAppointment(int appointmentId)
        {
            _response = _appoitmentService.UpdateAppointmentStatus(appointmentId,2);
            return Ok(_response);
        }

        /// <summary>
        /// Cancels the appointment having given appointment id
        /// </summary>
        /// <param name="appointmentId">Appointment Id</param>
        /// <returns></returns>
        [HttpPut("CancelAppointment")]
        public IActionResult CancelAppointment(int appointmentId)
        {
            _response = _appoitmentService.UpdateAppointmentStatus(appointmentId,3);
            return Ok(_response);
        }
    }
}
