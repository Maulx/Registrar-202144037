using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class CoursesRepository : Repository<Course>
    {
        public List<int> SessionsList() => ToList().Select(c => c.Session).Distinct().OrderBy(s => s).ToList();
        public SelectList NextSessionToSelectList => SelectListUtilities<Course>.Convert(ToList().Where(c => NextSession.ValidSessions.Contains(c.Session)).OrderBy(c => c.Session).ThenBy(c => c.Code), "Caption");
        public override bool Delete(int Id)
        {
            Course course = Get(Id);
            if (course != null)
            {
                course.DeleteAllRegistrations();
                course.DeleteAllAllocations();
            }
            return base.Delete(Id);
        }
        public bool Update(Course course, List<int> selectedStudentsId)
        {
            BeginTransaction();
            bool ok = base.Update(course);
            if (ok) course.UpdateRegistrations(selectedStudentsId);
            EndTransaction();
            return ok;
        }
    }
}
