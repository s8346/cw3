using System;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.DAL;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using AuthenticationSampleWebApp.DTOs;
using cw3.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using cw3.Services;
using cw3.ModelsNew;
using System.Diagnostics;

namespace cw3.Controllers
{

    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        //standardowe wypisanie
        //[HttpGet]
        //public string getStudent()
        //{
        //    return "Stachurski, Grzeszczak,Sobiczewska";
        //}

        //wstrzyknięcia
        private readonly IConfiguration _configuration;
        private readonly IDbService _dbService;
        private readonly ILoginService _loginService;
        private readonly IStudentsDbService _studentsDbService;
        private readonly s8346Context _studentsDbContext;

        public StudentsController(IDbService dbService, IConfiguration configuration,ILoginService loginService,IStudentsDbService studentsDbService, s8346Context context)
        {
            _loginService = loginService;
            _dbService = dbService;
            _configuration = configuration;
            _studentsDbService = studentsDbService;
            _studentsDbContext = context;
        }

        //zwaracanie wszystkich studentów
        //wykorzystanie SQLcommand oraz SQLconnnection
        /*[Authorize]
        [HttpGet]
        public IActionResult GetStudents([FromServices]IDbService dbService)
        {
            var listOfStudents = new List<Models.Student>();
            using (SqlConnection client = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;Integrated Security=True"))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "select IndexNumber, FirstName, LastName, BirthDate, Name, Semester, Student.Idenrollment, Studies.name " +
                        "from Student inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment " +
                        "inner join Studies on Enrollment.IdStudy = Studies.IdStudy";
                    //command.CommandText = "select * from DEPT";
                    client.Open();
                    SqlDataReader SqlDataReader = command.ExecuteReader();
                    while (SqlDataReader.Read())
                    {
                        var student = new Models.Student();
                        student.IndexNumber = SqlDataReader["IndexNumber"].ToString();
                        student.FirstName = SqlDataReader["FirstName"].ToString();
                        student.LastName = SqlDataReader["LastName"].ToString();
                        student.BirthDate = DateTime.Parse(SqlDataReader["BirthDate"].ToString());
                        student.Semester = int.Parse(SqlDataReader["Semester"].ToString());
                        student.IdEnrollment = int.Parse(SqlDataReader["Idenrollment"].ToString());
                        student.Studies = SqlDataReader["name"].ToString();
                        listOfStudents.Add(student);
                    }
                }
            }
            return Ok(listOfStudents);
        }*/

        //zwaracanie wszystkich studentów
        //wykorzystanie EF
        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_studentsDbService.GetStudents());
        }


        [HttpGet("{IndexNumber}")]
        public IActionResult getStudent(string IndexNumber)
        {
            int id = int.Parse(IndexNumber);
            Debug.WriteLine("My debug string here");
            using (SqlConnection client = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;Integrated Security=True"))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = " select IndexNumber, FirstName, LastName, BirthDate, IdEnrollment from Student where IndexNumber=@id";
                command.Parameters.AddWithValue("id", id);
                
                client.Open();
                SqlDataReader SqlDataReader = command.ExecuteReader();
                if (SqlDataReader.Read())
                {
                    var student = new Models.Student();
                    student.IndexNumber = SqlDataReader["IndexNumber"].ToString();
                    student.FirstName = SqlDataReader["FirstName"].ToString();
                    student.LastName = SqlDataReader["LastName"].ToString();
                    student.BirthDate = DateTime.Parse(SqlDataReader["BirthDate"].ToString());
                    student.IdEnrollment = int.Parse(SqlDataReader["IdEnrollment"].ToString());                    
                    return Ok(student);
                }
                return NotFound("Nie znaleziono studenta");

            }
        }

        [HttpGet("{IndexNumber}/{Semester}")]
        public IActionResult getSemester(string indexNumber, int semester)
        {
            int id = int.Parse(indexNumber);
            using (SqlConnection con = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;Integrated Security=True"))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = " select Semester, IdStudy, StartDate from Student inner join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment where Enrollment.Semester=@semester and Student.IndexNumber = @id;";
                com.Parameters.AddWithValue("id", id);
                com.Parameters.AddWithValue("semester", semester);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    Models.Enrollment enrollment = new Models.Enrollment();

                    enrollment.IdStudy = int.Parse(dr["IdStudy"].ToString());
                    enrollment.Semester = int.Parse(dr["Semester"].ToString());
                    enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    return Ok(enrollment);
                }
                return NotFound();
            }
        }



        //[HttpGet("{id}")]
        //public IActionResult GetStudent(int id)
        //{
        //    if (id == 1)
        //    {
        //        return Ok("Kowalski");
        //    }
        //    else if (id == 2)
        //    {
        //        return Ok("Stachurski");
        //    }

        //    return NotFound("Nie znaleziono studenta");
        //}

        //[HttpGet]
        //public string getStudents(string orderBy)
        //{
        //    return $"Stachurski, Grzeszczak,Sobiczewska sortowanie = {orderBy}";
        //}

        [HttpPost]
        public IActionResult CreateStudent(Models.Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        //        {
        //	"idStudent": 1,
        //	"firstName":"Filip",
        //	"lastName":"Stachurski"
        //}

        [HttpPut("{IndexNumber}")]
        public IActionResult PutStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Aktualizacja dokończona");
            }
            return NotFound("Nie znaleziono studenta o podanym id");

        }

        [HttpDelete("{IndexNumber}")]
        public IActionResult RemoveStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Usuwanie ukończone");
            }
            return NotFound("Nie znaleziono studenta o podanym id");
        }

        [Authorize]
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto requestDto)
        {
                return Ok(_loginService.Login(requestDto));
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("refresh-token/{rToken}")]
        public IActionResult RefreshToken(string rToken)
        {  
         return Ok(_loginService.RefreshToken(rToken));
        }

    }
}
