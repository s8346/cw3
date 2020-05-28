using Microsoft.AspNetCore.Mvc;
using cw3.DTOs.Requests;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }

        //[Authorize(Roles = "employee")]
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return StatusCode(201, _service.EnrollStudent(request)); 
        }

        //[Authorize(Roles = "employee")]
        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            return StatusCode(201, _service.PromoteStudents(request.Semester,request.Studies));
        }
    }
}