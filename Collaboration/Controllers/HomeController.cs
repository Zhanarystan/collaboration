using Collaboration.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Collaboration.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ProjectList(int? project_specification,
                                                    int? project_country)
        {
            var specificaitons = await db.Specifications.ToListAsync();
            ViewBag.Specifications = specificaitons;

            var countries = await db.Countries.ToListAsync();
            ViewBag.Countries = countries;
           
            var projects = await db.Projects.OrderByDescending(c => c.PostedDate).ToListAsync();

            var user_id = User.Identity.GetUserId();
            ViewBag.CurrentUser = db.Users.Find(user_id);
            if (project_specification != null && !project_specification.Equals("null"))
            {
                projects = projects.Where(c => c.SpecificationId == project_specification).ToList();
            }
            if(project_country!=null && !project_country.Equals("null")){
                Countries country = db.Countries.Find(project_country);
                projects = projects.Where(c => c.Countries.Contains(country)).ToList();
            }
            ViewBag.Projects = projects;
            return View();
        }

        

        [HttpGet]
        public ActionResult ProjectDetails(int? project_id)
        {
            Projects project = db.Projects.Find(project_id);
            ViewBag.Project = project;
            if (User.Identity.IsAuthenticated)
            {
                string user_id = User.Identity.GetUserId();
                EnrollmentRequests ers = db.EnrollmentRequests
                    .Where(c => c.ProjectId == project_id &&
                    c.UserId == user_id).SingleOrDefault();

                Enrollments er = db.Enrollments.Where(p => p.ProjectId == project_id && p.UserId == user_id).SingleOrDefault();
                ViewBag.Enrollment = er;
                ViewBag.EnrollmentRequest = ers;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectDetails(EnrollmentRequests ers)
        {
            if (string.IsNullOrEmpty(ers.RequestMessage))
            {
                ModelState.AddModelError("RequestMessage", "Message should be set");
            }
            else if (ers.RequestMessage.Length < 10)
            {
                ModelState.AddModelError("RequestMessage", "Message should contain at least 10 characters");
            }
            if (ModelState.IsValid)
            {
                db.EnrollmentRequests.Add(ers);
                db.SaveChanges();
                return Redirect("/Home/ProjectDetails?project_id=" + ers.ProjectId);
            }
            Projects project = db.Projects.Find(ers.ProjectId);
            ViewBag.Project = project;
            return View(ers);
        }

        [HttpPost]
        public ActionResult RequestProcessing(string? user_id, int? project_id, string? accept, string? reject)
        {
            EnrollmentRequests er = db.EnrollmentRequests.Where(p => p.ProjectId == project_id).
                Where(u => u.UserId == user_id).SingleOrDefault();

            Projects project = db.Projects.Find(project_id);
            if(er != null)
            {
                Enrollments enrollment = new Enrollments();
                enrollment.UserId = user_id;
                enrollment.ProjectId = project_id;

                if (accept != null)
                {
                    db.Enrollments.Add(enrollment);
                    db.EnrollmentRequests.Remove(er);
                }
                if (reject != null)
                {
                    db.EnrollmentRequests.Remove(er);
                }
            }
            
            db.SaveChanges();
            if (project.PostedById.Equals(User.Identity.GetUserId()))
            {
                return Redirect("/Manage/ProjectDetails?project_id=" + project_id);
            }
            return Redirect("/Home/ProjectDetails?project_id="+project_id);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult UserDetails(string? user_id)
        {
            ApplicationUser user = db.Users.Find(user_id);
            return View(user);
        }
    }
}