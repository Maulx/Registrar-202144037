using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class TeachersRepository : Repository<Teacher>
    {
        static Random rnd = new Random();
        public string GenerateUniqueCode()
        {
            string code;
            do { code = "CLG-420-" + rnd.Next(10000, 99999).ToString(); }
            while (ToList().Any(t => t.Code == code));
            return code;
        }
        public override bool Delete(int Id)
        {
            Teacher teacher = Get(Id);
            if (teacher != null) teacher.DeleteAllAllocations();
            return base.Delete(Id);
        }
        public bool Update(Teacher teacher, List<int> selectedCoursesId)
        {
            BeginTransaction();
            bool ok = base.Update(teacher);
            if (ok) teacher.UpdateAllocations(selectedCoursesId);
            EndTransaction();
            return ok;
        }
    }
}
