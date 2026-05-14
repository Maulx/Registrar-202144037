using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class StudentsRepository : Repository<Student>
    {
        static Random rnd = new Random();
        public List<int> StudentsYearsList() => ToList().Select(s => s.Year).Distinct().OrderByDescending(y => y).ToList();
        public string GenerateUniqueCode()
        {
            string code;
            do { code = DateTime.Now.Year.ToString() + rnd.Next(100000, 999999).ToString(); }
            while (ToList().Any(s => s.Code == code));
            return code;
        }
        public override bool Delete(int Id)
        {
            Student student = Get(Id);
            if (student != null) student.DeleteAllRegistrations();
            return base.Delete(Id);
        }
        public bool Update(Student student, List<int> selectedCoursesId)
        {
            BeginTransaction();
            bool ok = base.Update(student);
            if (ok) student.UpdateRegistrations(selectedCoursesId);
            EndTransaction();
            return ok;
        }
    }
}
