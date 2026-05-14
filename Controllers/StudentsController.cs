using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace Controllers
{
    [UserAccess(Models.Access.View)]
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

        private void ResetCurrentStudentInfo()
        {
            Session["CurrentStudentId"] = 0;
        }

        public ActionResult List()
        {
            InitSessionVariables();
            ResetCurrentStudentInfo();
            return View();
        }

        public ActionResult ToggleSearch()
        {
            InitSessionVariables();
            Session["Search"] = !(bool)Session["Search"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            InitSessionVariables();
            Session["SearchString"] = value != null ? value.ToLower() : "";
            return RedirectToAction("List");
        }

        public ActionResult SetSearchYear(int value)
        {
            InitSessionVariables();
            Session["SelectedStudentYear"] = value;
            return RedirectToAction("List");
        }

        public ActionResult GetStudents(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                IEnumerable<Student> result = DB.Students.ToList();

                bool search = (bool)Session["Search"];
                string searchString = (string)Session["SearchString"];
                int selectedYear = (int)Session["SelectedStudentYear"];

                if (search)
                {
                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        result = result.Where(s =>
                            s.Code.ToLower().Contains(searchString) ||
                            s.FirstName.ToLower().Contains(searchString) ||
                            s.LastName.ToLower().Contains(searchString));
                    }

                    if (selectedYear != 0)
                    {
                        result = result.Where(s => s.Year == selectedYear);
                    }
                }

                result = result.OrderByDescending(s => s.Year)
                               .ThenBy(s => s.LastName)
                               .ThenBy(s => s.FirstName);

                return PartialView(result);
            }
            catch (Exception ex)
            {
                return Content("Erreur interne : " + ex.Message, "text/html");
            }
        }

        public ActionResult Details(int id)
        {
            Session["CurrentStudentId"] = id;

            Student student = DB.Students.Get(id);

            if (student != null)
                return View(student);

            return RedirectToAction("List");
        }
        [UserAccess(Access.Admin)]
        public ActionResult Edit()
        {
            int id = (int)Session["id"];
            Student student = DB.Students.Get(id);
            if (student != null)
            {
                ViewBag.Registrations = student.NextSessionCoursesToSelectList;
                ViewBag.Courses = DB.Courses.NextSessionToSelectList;
                return View(DB.Students.Get(id));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [UserAccess(Access.Admin)]
        public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            if (student.IsValid ())
            {
                student.Id = (int)Session["id"];
                student.Code = (string)Session["code"];
                DB.Students.Update(student, selectedCoursesId);
                return RedirectToAction("Details", new { id = student.Id });
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }
    }
}