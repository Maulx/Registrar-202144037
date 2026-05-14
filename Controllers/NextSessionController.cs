using System;
using System.Web.Mvc;
using Models;

public class NextSessionController : Controller
{
    public ActionResult Edit()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Edit(int year, string session)
    {
        int month = session == "Automne" ? 8 : 1;

        NextSession.CurrentDate = new DateTime(year, month, 2);

        return RedirectToAction("Index", "Students");
    }
}