using DAL;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class StudentsRepository : Repository<Student>
    {
        public List<int> StudentsYearsList()
        {
            List<int> years = new List<int>();

            foreach (Student student in ToList().OrderByDescending(s => s.Year))
            {
                if (!years.Contains(student.Year))
                    years.Add(student.Year);
            }

            return years;
        }

        public override bool Delete(int Id)
        {
            Student student = Get(Id);

            if (student != null)
            {
                // À activer plus tard quand Registration existe
                // student.DeleteAllRegistrations();
            }

            return base.Delete(Id);
        }
    }
}