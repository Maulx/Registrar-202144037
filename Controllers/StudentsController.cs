using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace Controllers
{
    [UserAccess(Access.View)]
    public class StudentsController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CurrentStudentId"] == null) Session["CurrentStudentId"] = 0;
            if (Session["Search"] == null) Session["Search"] = false;
            if (Session["SearchString"] == null) Session["SearchString"] = "";
            if (Session["SelectedStudentYear"] == null) Session["SelectedStudentYear"] = 0;
            Session["StudentsYearsList"] = DB.Students.StudentsYearsList();
        }
        public ActionResult Index() => RedirectToAction("List");
        public ActionResult List(){ InitSessionVariables(); Session["CurrentStudentId"] = 0; return View(); }
        public ActionResult ToggleSearch(){ InitSessionVariables(); Session["Search"] = !(bool)Session["Search"]; return RedirectToAction("List"); }
        public ActionResult SetSearchString(string value){ InitSessionVariables(); Session["SearchString"] = value != null ? value.ToLower() : ""; return RedirectToAction("List"); }
        public ActionResult SetSearchYear(int value){ InitSessionVariables(); Session["SelectedStudentYear"] = value; return RedirectToAction("List"); }
        public ActionResult GetStudents(bool forceRefresh = false)
        {
            InitSessionVariables();
            IEnumerable<Student> result = DB.Students.ToList();
            if ((bool)Session["Search"])
            {
                string q = (string)Session["SearchString"];
                int year = (int)Session["SelectedStudentYear"];
                if (!string.IsNullOrWhiteSpace(q)) result = result.Where(s => s.Code.ToLower().Contains(q) || s.FirstName.ToLower().Contains(q) || s.LastName.ToLower().Contains(q));
                if (year != 0) result = result.Where(s => s.Year == year);
            }
            return PartialView(result.OrderByDescending(s => s.Year).ThenBy(s => s.LastName).ThenBy(s => s.FirstName));
        }
        public ActionResult Details(int id){ InitSessionVariables(); Session["CurrentStudentId"] = id; Student student = DB.Students.Get(id); return student != null ? View(student) : (ActionResult)RedirectToAction("List"); }
        [UserAccess(Access.Write)] public ActionResult Create()
        {
            ViewBag.Creating = true; ViewBag.Session = NextSession.Caption;
            return View("StudentForm", new Student { Code = DB.Students.GenerateUniqueCode(), BirthDate = DateTime.Today });
        }
        [HttpPost][UserAccess(Access.Write)] public ActionResult Create(Student student)
        {
            student.Code = DB.Students.GenerateUniqueCode(); DB.Students.Add(student); return RedirectToAction("Details", new { id = student.Id });
        }
        [UserAccess(Access.Write)] public ActionResult Edit(int? id)
        {
            int currentId = id ?? (int)Session["CurrentStudentId"]; Session["CurrentStudentId"] = currentId;
            Student student = DB.Students.Get(currentId); if (student == null) return RedirectToAction("List");
            ViewBag.Creating = false; ViewBag.Session = NextSession.Caption; ViewBag.Registrations = student.NextSessionCoursesToSelectList; ViewBag.Courses = DB.Courses.NextSessionToSelectList;
            return View("StudentForm", student);
        }
        [HttpPost][UserAccess(Access.Write)] public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            Student old = DB.Students.Get(student.Id); if (old == null) return RedirectToAction("List");
            student.Code = old.Code; DB.Students.Update(student, selectedCoursesId); return RedirectToAction("Details", new { id = student.Id });
        }
        [UserAccess(Access.Write)] public ActionResult Delete(int? id)
        {
            int currentId = id ?? (int)Session["CurrentStudentId"]; DB.Students.Delete(currentId); return RedirectToAction("List");
        }
    }
}
