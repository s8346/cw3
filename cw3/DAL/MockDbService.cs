using cw3.Models;
using System.Collections.Generic;

namespace cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IndexNumber="1", FirstName="Jan", LastName="Kowalski"},
                new Student{IndexNumber="2", FirstName="Alina", LastName="Kowalska"},
                new Student{IndexNumber="3", FirstName="Patryk", LastName="Lepszczak"}
            };
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

    }
}
