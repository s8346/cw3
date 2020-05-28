using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.ModelsNew;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private readonly s8346Context _dbContext;

        public SqlServerDbService(s8346Context context)
        {
            this._dbContext = context;
        }

        public SqlServerDbService()
        {
        }

        public IEnumerable<Student> GetStudents()
        {
            return _dbContext.Student.ToList();
        }

        //zastosowanie SQLConnection oraz SQLCommand
        /*
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;User ID=apbds8346;Password=admin'"))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    int idEnrollment;
                    command.Connection = connection;
                    connection.Open();
                    var tran = connection.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        command.CommandText = "select IdStudy from studies where name=@name";
                        command.Parameters.AddWithValue("name", request.Studies);
                        var SqlDataReader = command.ExecuteReader();
                        int id;
                        if (!SqlDataReader.Read())
                        {
                            SqlDataReader.Close();
                            command.CommandText = "select idStudy from Studies where idStudy=(select max(IdStudy) from studies);";
                            SqlDataReader = command.ExecuteReader();
                            if (SqlDataReader.Read())
                            {
                                id = int.Parse(SqlDataReader["IdStudy"].ToString())+1;
                                SqlDataReader.Close();
                                command.CommandText = "Insert Into Studies (IdStudy,Name) values (@id,@name)";
                                command.Parameters.AddWithValue("id", id);
                                command.Parameters.AddWithValue("name", request.Studies);
                                command.ExecuteNonQuery();
                            }
                        }

                        SqlDataReader.Close();
                        command.CommandText = "select Enrollment.idStudy from Enrollment inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                            "where Studies.name=@name and Enrollment.Semester=1";
                        command.Parameters.AddWithValue("name", request.Studies);
                        SqlDataReader = command.ExecuteReader();

                        if (!SqlDataReader.Read())
                        {
                            SqlDataReader.Close();
                            command.CommandText = "select idEnrollment from Enrollment where idEnrollment = (select max(idEnrollment) from Enrollment)";
                            SqlDataReader = command.ExecuteReader();
                            
                            if (SqlDataReader.Read())
                            {
                                idEnrollment = int.Parse(SqlDataReader["idEnrollment"].ToString())+1;
                                SqlDataReader.Close();
                                command.CommandText = "select idStudy from Enrollment inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                                "where Studies.name=@name";
                                SqlDataReader = command.ExecuteReader();
                                int idStudy;
                                if (SqlDataReader.Read())
                                {
                                idStudy= int.Parse(SqlDataReader["idStudy"].ToString()) + 1;
                                SqlDataReader.Close();
                                command.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) values (@IdEnrollment,1,@IdStudy,@StartDate)";
                                command.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                                command.Parameters.AddWithValue("StartDate", DateTime.Today);
                                command.Parameters.AddWithValue("IdStudy", idStudy);
                                command.ExecuteReader();

                                SqlDataReader.Close();
                                command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @fname, @lname, @birthDate, @idEnrollment";
                                command.Parameters.AddWithValue("index", request.IndexNumber);
                                command.Parameters.AddWithValue("fname", request.FirstName);
                                command.Parameters.AddWithValue("lname", request.LastName);
                                command.Parameters.AddWithValue("birthDate", request.BirthDate);
                                command.Parameters.AddWithValue("idEnrollment", idEnrollment);
                                command.ExecuteNonQuery();
                                }
                            }
                        }
                        tran.Commit();
                    }
                    catch (SqlException sql)
                    {
                        tran.Rollback();
                        Console.Write(sql);
                    }
                }
            }
            var response = new EnrollStudentResponse()
            {
                LastName = request.LastName,
                Semester = 1,
                StartDate = DateTime.Now
            };

            return response;
        }*/

        //zastosowanie EF
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {   
                if (_dbContext.Studies.FirstOrDefault(st => st.Name == request.Studies) == null)
                { return null; }

                int idStudy = _dbContext.Studies.Where(st => st.Name == request.Studies).Select(st => st.IdStudy).SingleOrDefault();

                Enrollment enrollment = _dbContext.Enrollment.FirstOrDefault(e => (e.IdStudy == idStudy) && (e.Semester == 1));

                if (enrollment == null)
                {
                    int maxId = _dbContext.Enrollment.Max(e => e.IdEnrollment);

                    enrollment = new Enrollment();
                    enrollment.IdEnrollment = maxId + 1;
                    enrollment.Semester = 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = DateTime.Now;
                    _dbContext.Enrollment.Add(enrollment);
                }

                var StudentExist = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(request.IndexNumber));
                if (StudentExist != null)
                { return null; }

                //"BirthDate": "1993-03-30"
                Student student = new Student();
                student.IndexNumber = request.IndexNumber;
                student.FirstName = request.FirstName;
                student.LastName = request.LastName;
                student.BirthDate = request.BirthDate;
                student.IdEnrollment = enrollment.IdEnrollment;
                _dbContext.Student.Add(student);

                _dbContext.SaveChanges();
                transaction.Commit();

                var response = new EnrollStudentResponse()
                {
                    LastName = request.LastName,
                    Semester = 1,
                    StartDate = DateTime.Now
                };

                return response;
            }
        }

        //zastosowanie SQLConnection oraz SQLCommand
        /*
        public Enrollment PromoteStudents(int semester, string studiesName)
        {
            var enrollment = new Enrollment();
            using (var connection = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;User ID=apbds8346;Password=admin'"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    command.CommandText = "SELECT semester FROM Enrollment inner join Studies on Enrollment.IdStudy=Studies.idStudy where Enrollment.Semester = @semester AND Studies.name = (SELECT name from Studies where Name = @name)";
                    command.Parameters.AddWithValue("name", studiesName);
                    command.Parameters.AddWithValue("semester", semester);

                    var SqlDataReader = command.ExecuteReader();

                    if (!SqlDataReader.Read())
                    {
                        //return NotFound();
                    }
                    SqlDataReader.Close();
                    command.Parameters.Clear();

                    command.CommandText = "PromoteStudents";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Studies", studiesName);
                    command.Parameters.AddWithValue("Semester", semester);

                    SqlDataReader = command.ExecuteReader();
                    if (SqlDataReader.Read())
                    {
                        enrollment.IdEnrollment = int.Parse(SqlDataReader["IdEnrollment"].ToString());
                        enrollment.IdStudy = int.Parse(SqlDataReader["IdStudy"].ToString());
                        enrollment.Semester = int.Parse(SqlDataReader["Semester"].ToString());
                        enrollment.StartDate = DateTime.Parse(SqlDataReader["StartDate"].ToString());
                    }
                }
            }
            return enrollment;
        }
        }*/

        //zastosowanie EF
        public PromoteStudentsResponse PromoteStudents(int semester, string studies)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                int idStudy = _dbContext.Studies
                .Where(st => st.Name == studies)
                .Select(st => st.IdStudy).SingleOrDefault();

                Enrollment enrollment = _dbContext.Enrollment.FirstOrDefault(e => e.IdStudy == idStudy && e.Semester == semester);

                if (enrollment == null)
                {
                    return null;
                }

                int oldIdEnrollment = enrollment.IdEnrollment;
                enrollment = _dbContext.Enrollment.FirstOrDefault(e => e.IdStudy == idStudy && e.Semester == semester + 1);

                if (enrollment == null)
                {
                    int maxId = _dbContext.Enrollment.Max(e => e.IdEnrollment);
                    enrollment = new Enrollment();
                    enrollment.IdEnrollment = maxId + 1;
                    enrollment.Semester = semester + 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = DateTime.Now;
                    _dbContext.Enrollment.Add(enrollment);
                    _dbContext.SaveChanges();
                  
                }

                var students = _dbContext.Student.Where(s => s.IdEnrollment == oldIdEnrollment).ToList();

                foreach (Student student in students)
                {
                    student.IdEnrollment = enrollment.IdEnrollment;
                }

                _dbContext.SaveChanges();
                transaction.Commit();
                var response = new PromoteStudentsResponse()
                {
                    Study = studies,
                    NewIdStudy =enrollment.IdStudy,
                    NewSemester = enrollment.Semester
                };

                return response;
            }
        }

        //zastosowanie EF
        public string UpdateStudent(UpdateStudentRequest request)
        {
            var student = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(request.IndexNumber));
            if (student == null)
            {
                return "Student not found";
            }
               
            student.FirstName = request.FirstName != null ? request.FirstName : student.FirstName;
            student.LastName = request.LastName != null ? request.LastName : student.LastName;
            student.BirthDate = request.BirthDate != null ? request.BirthDate : student.BirthDate;
            _dbContext.Update(student);
            _dbContext.SaveChanges();
            return "OK";
        }

        //zastosowanie EF
        public string DeleteStudent(string index)
        {
            if (index == null)
            {
                return "Bad student index";
            }

            var student = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(index));
            if (student == null)
                return "Student not found";

            _dbContext.Remove(student);
            _dbContext.SaveChanges();
            return "OK";
        }

        //sprawdzanie istnienie indeksu w bazie - cw7
        public bool CheckIndex(string IndexNumber)
        {
            int id = int.Parse(IndexNumber);
            using (SqlConnection client = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;User ID=apbds8346;Password=admin'"))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = " select IndexNumber, FirstName, LastName, BirthDate, IdEnrollment from Student where IndexNumber=@id";
                command.Parameters.AddWithValue("id", id);

                client.Open();
                SqlDataReader SqlDataReader = command.ExecuteReader();
                if (SqlDataReader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
