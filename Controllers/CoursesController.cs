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
    public class CoursesController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CurrentCourseId"] == null)
                Session["CurrentCourseId"] = 0;

            if (Session["Search"] == null)
                Session["Search"] = false;

            if (Session["SearchString"] == null)
                Session["SearchString"] = "";

            Session["CoursesSessionsList"] = DB.Courses.SessionsList();
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            InitSessionVariables();
            Session["CurrentCourseId"] = 0;

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

            Session["SearchString"] =
                value != null ? value.ToLower() : "";

            return RedirectToAction("List");
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            InitSessionVariables();

            IEnumerable<Course> result = DB.Courses.ToList();

            string q = (string)Session["SearchString"];

            if ((bool)Session["Search"] &&
                !string.IsNullOrWhiteSpace(q))
            {
                result = result.Where(c =>
                    c.Code.ToLower().Contains(q) ||
                    c.Title.ToLower().Contains(q));
            }

            return PartialView(
                result
                    .OrderBy(c => c.Session)
                    .ThenBy(c => c.Code)
            );
        }

        public ActionResult Details(int id)
        {
            InitSessionVariables();

            Session["CurrentCourseId"] = id;

            Course course = DB.Courses.Get(id);

            return course != null
                ? View(course)
                : (ActionResult)RedirectToAction("List");
        }

        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            ViewBag.Creating = true;

            return View("CourseForm", new Course());
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Create(Course course)
        {
            DB.Courses.Add(course);

            return RedirectToAction(
                "Details",
                new { id = course.Id }
            );
        }

        [UserAccess(Access.Write)]
        public ActionResult Edit(int? id)
        {
            int currentId =
                id ?? (int)Session["CurrentCourseId"];

            Session["CurrentCourseId"] = currentId;

            Course course = DB.Courses.Get(currentId);

            if (course == null)
                return RedirectToAction("List");

            ViewBag.Creating = false;
            ViewBag.Session = NextSession.Caption;

            ViewBag.Students = course.StudentsToSelectList;

            ViewBag.AllStudents =
                SelectListUtilities<Student>.Convert(
                    DB.Students
                        .ToList()
                        .OrderBy(s => s.LastName)
                        .ThenBy(s => s.FirstName),
                    "Caption"
                );

            return View("CourseForm", course);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            DB.Courses.Update(course, selectedStudentsId);
            return RedirectToAction("Details", new { id = course.Id });
        }

        [UserAccess(Access.Write)]
        public ActionResult Delete(int? id)
        {
            int currentId =
                id ?? (int)Session["CurrentCourseId"];

            DB.Courses.Delete(currentId);

            return RedirectToAction("List");
        }
    }
}