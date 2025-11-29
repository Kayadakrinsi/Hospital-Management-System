using HMSBAL.Interfaces.Masters;
using HMSMAL.Common;
using HMSMAL.DTO.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CLAppointmentsController : ControllerBase
    {
        IAppointmentsService _appoitmentService { get; set; }
        Response _response;

        public CLAppointmentsController(IAppointmentsService appoitmentService)
        {
            _appoitmentService = appoitmentService;
            _response = new();

        }

        [HttpGet("GetAppointments")]
        public IActionResult GetAppointments(int patientId = 0, int doctorId = 0)
        {
            _response = _appoitmentService.GetAppointments(patientId, doctorId);
            return Ok(_response);
        }

        [HttpGet("GetDoctors")]
        public IActionResult GetDoctors()
        {
            _response = _appoitmentService.GetDoctors();
            return Ok(_response);
        }

        [HttpGet("GetPatients")]
        public IActionResult GetPatients()
        {
            _response = _appoitmentService.GetPatients();
            return Ok(_response);
        }

        //[Authorize(Roles = "Admin,Receptionist,Patient")]
        [HttpPost("BookAppointment")]
        public IActionResult BookAppointment(DTOAppointment objDTOAppointment)
        {
            _response = _appoitmentService.BookAppointment(objDTOAppointment);
            return Ok(_response);
        }

        //[Authorize(Roles = "Admin,Doctor")]
        [HttpPut("CompleteAppointment")]
        public IActionResult CompleteAppointment(int appointmentId)
        {
            _response = _appoitmentService.UpdateAppointmentStatus(appointmentId,2);
            return Ok(_response);
        }

        //[Authorize(Roles = "Admin,Receptionist,Patient")]
        [HttpPut("CancelAppointment")]
        public IActionResult CancelAppointment(int appointmentId)
        {
            _response = _appoitmentService.UpdateAppointmentStatus(appointmentId,3);
            return Ok(_response);
        }
    }
}
