using myBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myBlog.Controllers
{
    public class HomeController : Controller
    {

        myBlogPrEntities1 db = new myBlogPrEntities1();

        public ActionResult Index()
        {
            var projectList = db.Projects.OrderByDescending(x => x.CreatedDate).ToList();
            var aboutInfo = db.About.FirstOrDefault();
            ViewBag.About = aboutInfo;

            return View(projectList);
        }

        // Linkten gelen ID'yi (örn: /Home/ProjectDetails/5) parametre olarak alır
        public ActionResult ProjectDetails(int id)
        {
            var project = db.Projects.Find(id);
            return View(project);
        }

        public ActionResult About()
        {
            var about = db.About.FirstOrDefault(); 
            return View(about);
        }

        public ActionResult EditAbout()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "Account");

            var about = db.About.FirstOrDefault();
            return View(about);
        }

        [HttpPost]
        public ActionResult EditAbout(About updatedAbout, HttpPostedFileBase imageFile)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "Account");

            var about = db.About.FirstOrDefault();

            if (about != null)
            {
                // 1. Yazı bilgilerini güncelle
                about.FullName = updatedAbout.FullName;
                about.ShortDescription = updatedAbout.ShortDescription;
                about.LongDescription = updatedAbout.LongDescription;
                about.GithubLink = updatedAbout.GithubLink;
                about.LinkedInLink = updatedAbout.LinkedInLink;

                // 2. Resim seçilmişse onu kaydet
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Dosya ismini al
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    // Resimler klasörüne kaydet
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    imageFile.SaveAs(path);

                    // Veritabanına yolunu yaz
                    about.ProfileImageUrl = "/Content/images/" + fileName;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index"); // İşlem bitince ana sayfaya dön
        }


        public ActionResult Projects()
        {
            // Veritabanından projeleri çekip sayfaya gönderiyoruz
            var projects = db.Projects.OrderByDescending(x => x.CreatedDate).ToList();
            return View(projects);
        }

        public ActionResult Blog()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
    }
}