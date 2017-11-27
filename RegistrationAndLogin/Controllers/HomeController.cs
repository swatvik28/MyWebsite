using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationAndLogin.Models;
using System.Web.UI;

namespace RegistrationAndLogin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]         // Since onlyauthorized users can access this home page.
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListClasses()
        {
            var democls = new List<DemoClass>();
            using (RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities())
            democls = RE.DemoClasses.ToList();
            return View(democls);
        }

        
        public ActionResult Edit(int id = 0)
        {
            RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities();
            DemoClass rec = RE.DemoClasses.Find(id);
            if(rec == null) 
            {
               return HttpNotFound();
            }
            return View(rec);

        }

        [HttpPost]
        public ActionResult Edit(DemoClass rec)
        {
            if (ModelState.IsValid)
            {
                RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities();
                RE.Entry(rec).State = System.Data.Entity.EntityState.Modified;
                RE.SaveChanges();
                string message = "Class details has been updated successfully.";
                TempData["successMessage"] = message;
                return RedirectToAction("ListClasses", "Home");
               
            }
            return View(rec);
            
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities();
            var rec = RE.DemoClasses.Find(id);
            if (rec == null)
            {
                return HttpNotFound();
            }
            return View(rec);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities();
            var rec = RE.DemoClasses.Find(id);
            RE.DemoClasses.Remove(rec);
            RE.SaveChanges();
            string message = "Class has been deleted successfully.";
            TempData["successMessage"] = message;
            return RedirectToAction("ListClasses");
            
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DemoClass rec)
        {
            RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities();
            if(ModelState.IsValid)
            {
                RE.DemoClasses.Add(rec);
                RE.SaveChanges();
                string message = "Class has been created successfully.";
                TempData["successMessage"] = message;
                return RedirectToAction("ListClasses");
            }           
            return View(rec);
        }
    }
}