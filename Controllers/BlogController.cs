using myBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;

namespace myBlog.Controllers
{
    public class BlogController : Controller
    {
        myBlogPrEntities1 db = new myBlogPrEntities1();

        // 1. BLOG LİSTESİ (Herkes görebilir)
        public ActionResult Index()
        {
            // Blogları en yeniden eskiye doğru sırala
            var blogs = db.Blogs.OrderByDescending(x => x.CreatedDate).ToList();
            return View(blogs);
        }

        // 2. BLOG DETAY (Yazıyı Oku)
        public ActionResult Details(int id)
        {
            var blog = db.Blogs.Find(id);
            if (blog == null) return HttpNotFound();
            return View(blog);
        }

        // 3. YENİ YAZI EKLEME SAYFASI (Sadece Admin)
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["admin"] == null) return RedirectToAction("Login", "Account");
            return View();
        }

        // 4. YENİ YAZIYI KAYDETME (POST)
        [HttpPost]
        public ActionResult Create(Blogs b, HttpPostedFileBase imageFile)
        {
            if (Session["admin"] == null) return RedirectToAction("Login", "Account");

            try
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueName = Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), uniqueName);
                    imageFile.SaveAs(path);
                    b.ImageUrl = "/Content/images/" + uniqueName;
                }
                else
                {
                    b.ImageUrl = "https://via.placeholder.com/800x400"; // Varsayılan resim
                }

                b.CreatedDate = DateTime.Now;
                db.Blogs.Add(b);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Error = "Bir hata oluştu.";
                return View();
            }
        }

        // 5. SİLME (Sadece Admin)
        public ActionResult Delete(int id)
        {
            if (Session["admin"] == null) return RedirectToAction("Login", "Account");

            var blog = db.Blogs.Find(id);
            if (blog != null)
            {
                // Varsa resim dosyasını da siliyoruz
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    string fullPath = Request.MapPath(blog.ImageUrl);
                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                }

                db.Blogs.Remove(blog);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}