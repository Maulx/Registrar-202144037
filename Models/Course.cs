using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }

        [JsonIgnore] public string Caption => "[" + Session + "] " + Code + " " + Title;
        [JsonIgnore] public List<Registration> Registrations => DB.Registrations.ToList().Where(r => r.CourseId == Id).ToList();
        [JsonIgnore] public List<Registration> NextSessionRegistrations => DB.Registrations.ToList().Where(r => r.CourseId == Id && r.IsNextSession).ToList();
        [JsonIgnore] public List<Allocation> Allocations => DB.Allocations.ToList().Where(a => a.CourseId == Id).ToList();
        [JsonIgnore] public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(a => a.CourseId == Id && a.IsNextSession).ToList();
        [JsonIgnore] public Teacher NextSessionTeacher => NextSessionAllocations.FirstOrDefault()?.Teacher;
        [JsonIgnore] public SelectList StudentsToSelectList => SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");
        [JsonIgnore] public List<Student> NextSessionStudents => NextSessionRegistrations.Select(r => r.Student).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();

        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations.ToList()) DB.Registrations.Delete(registration.Id);
        }
        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations.ToList()) DB.Allocations.Delete(allocation.Id);
        }
        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations.ToList()) DB.Registrations.Delete(registration.Id);
        }
        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteAllRegistrations();
            if (selectedStudentsId != null)
                foreach (int studentId in selectedStudentsId)
                    DB.Registrations.Add(new Registration { StudentId = studentId, CourseId = Id });
        }
    }
}
