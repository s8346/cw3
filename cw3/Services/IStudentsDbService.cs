using System;
using System.Collections.Generic;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.ModelsNew;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollStudentRequest);
        PromoteStudentsResponse PromoteStudents(int semester, string studies);
        public IEnumerable<Student> GetStudents();
        public string UpdateStudent(UpdateStudentRequest request);
        public string DeleteStudent(string index);
        public bool CheckIndex(string index);
    }
}
