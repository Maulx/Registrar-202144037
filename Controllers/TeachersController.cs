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
    public class TeachersController : Controller
    {
        private void InitSessionVariables(){ if(Session["CurrentTeacherId"]==null)Session["CurrentTeacherId"]=0; if(Session["Search"]==null)Session["Search"]=false; if(Session["SearchString"]==null)Session["SearchString"]=""; }
        public ActionResult Index()=>RedirectToAction("List");
        public ActionResult List(){ InitSessionVariables(); Session["CurrentTeacherId"]=0; return View(); }
        public ActionResult ToggleSearch(){ InitSessionVariables(); Session["Search"] = !(bool)Session["Search"]; return RedirectToAction("List"); }
        public ActionResult SetSearchString(string value){ InitSessionVariables(); Session["SearchString"] = value != null ? value.ToLower() : ""; return RedirectToAction("List"); }
        public ActionResult GetTeachers(bool forceRefresh=false)
        { InitSessionVariables(); IEnumerable<Teacher> result=DB.Teachers.ToList(); string q=(string)Session["SearchString"]; if((bool)Session["Search"]&&!string.IsNullOrWhiteSpace(q)) result=result.Where(t=>t.Code.ToLower().Contains(q)||t.FirstName.ToLower().Contains(q)||t.LastName.ToLower().Contains(q)); return PartialView(result.OrderBy(t=>t.LastName).ThenBy(t=>t.FirstName)); }
        public ActionResult Details(int id){ InitSessionVariables(); Session["CurrentTeacherId"]=id; Teacher teacher=DB.Teachers.Get(id); return teacher!=null?View(teacher):(ActionResult)RedirectToAction("List"); }
        [UserAccess(Access.Write)] public ActionResult Create(){ ViewBag.Creating=true; return View("TeacherForm", new Teacher{Code=DB.Teachers.GenerateUniqueCode(),StartDate=DateTime.Today}); }
        [HttpPost][UserAccess(Access.Write)] public ActionResult Create(Teacher teacher){ teacher.Code=DB.Teachers.GenerateUniqueCode(); DB.Teachers.Add(teacher); return RedirectToAction("Details", new{id=teacher.Id}); }
        [UserAccess(Access.Write)] public ActionResult Edit(int? id){ int currentId=id??(int)Session["CurrentTeacherId"]; Session["CurrentTeacherId"]=currentId; Teacher teacher=DB.Teachers.Get(currentId); if(teacher==null)return RedirectToAction("List"); ViewBag.Creating=false; ViewBag.Session=NextSession.Caption; ViewBag.Allocations=teacher.NextSessionCoursesToSelectList; ViewBag.Courses=DB.Courses.NextSessionToSelectList; return View("TeacherForm", teacher); }
        [HttpPost][UserAccess(Access.Write)] public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId){ Teacher old=DB.Teachers.Get(teacher.Id); if(old==null)return RedirectToAction("List"); teacher.Code=old.Code; DB.Teachers.Update(teacher, selectedCoursesId); return RedirectToAction("Details", new{id=teacher.Id}); }
        [UserAccess(Access.Write)] public ActionResult Delete(int? id){ int currentId=id??(int)Session["CurrentTeacherId"]; DB.Teachers.Delete(currentId); return RedirectToAction("List"); }
    }
}
