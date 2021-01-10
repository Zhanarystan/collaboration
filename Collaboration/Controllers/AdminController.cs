using Collaboration.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Collaboration.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            IEnumerable<Countries> countries = db.Countries.ToList();
            return View(countries);
        }

        [HttpGet]
        public ActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCountry(CountriesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Countries country = new Countries();
                country.Name = model.Name;
                country.Code = model.Code;
                db.Countries.Add(country);
                db.SaveChanges();
            }
            return Redirect("/Admin/Index");
        }

        [HttpGet]
        public ActionResult EditCountry(int? id)
        {
            Countries country = db.Countries.Find(id);

            return View(country);
        }

        [HttpPost]
        public ActionResult EditCountry(Countries country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/Admin/Index");
            }
            return View(country);
        }

        [HttpGet]
        public ActionResult DeleteCountry(int? id)
        {
            Countries c = db.Countries.Find(id);
            db.Countries.Remove(c);
            db.SaveChanges();
            return Redirect("/Admin/Index");
        }

        [HttpGet]
        public ActionResult Specifications()
        {
            IEnumerable<Specifications> s = db.Specifications.ToList();
            return View(s);
        }

        [HttpGet]
        public ActionResult CreateSpecification()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSpecification(Specifications model)
        {
            if (ModelState.IsValid)
            {
                db.Specifications.Add(model);
                db.SaveChanges();
                return Redirect("/Admin/Index");
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult EditSpecification(int? id)
        {
            Specifications s = db.Specifications.Find(id);

            return View(s);
        }

        [HttpPost]
        public ActionResult EditSpecification(Specifications s)
        {
            if (ModelState.IsValid)
            {
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();

                return Redirect("/Admin/Specifications");
            }

            return View(s);
        }

        [HttpGet]
        public ActionResult DeleteSpecification(int? id)
        {
            Specifications s = db.Specifications.Find(id);
            db.Specifications.Remove(s);
            db.SaveChanges();
            return Redirect("/Admin/Specifications");
        }

        [HttpGet]
        public ActionResult Projects()
        {
            IEnumerable<Projects> p = db.Projects.ToList();

            return View(p);
        }

        [HttpGet]
        public ActionResult DeleteProject(int? id)
        {
            Projects p = db.Projects.Find(id);
            db.Projects.Remove(p);
            db.SaveChanges();
            return Redirect("/Admin/Projects");
        }

        [HttpGet]
        public ActionResult DetailsProject(int? id)
        {
            Projects p = db.Projects.Find(id);
            return View(p);
        }
    }
}