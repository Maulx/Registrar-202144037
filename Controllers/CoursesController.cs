using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrar.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        // GET: Courses/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Courses/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Courses/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
