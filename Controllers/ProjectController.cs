using System;
using System.Collections.Generic;
using System.IO; // Dosya işlemleri için gerekli
using System.Linq;
using System.Web;
using System.Web.Mvc;
using myBlog.Models;

namespace myBlog.Controllers
{
    public class ProjectController : Controller
    {
        myBlogPrEntities1 db = new myBlogPrEntities1();

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Projects p, HttpPostedFileBase imageFile)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                // Resim seçilmiş mi kontrol et
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Resim için benzersiz bir isim oluştur (aynı isimli resimler çakışmasın)
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueName = Guid.NewGuid().ToString() + extension;

                    // Resmi sunucuda kaydedeceğimiz klasör yolu
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), uniqueName);

                    // Resmi klasöre kaydet
                    imageFile.SaveAs(path);

                    // Veritabanına resmin YOLUNU yaz
                    p.ImageUrl = "/Content/images/" + uniqueName;
                }
                else
                {
                    // Resim seçilmediyse varsayılan bir resim ata
                    p.ImageUrl = "https://via.placeholder.com/300";
                }

                // Tarihi otomatik ata
                p.CreatedDate = DateTime.Now;

                // Veritabanına ekle ve kaydet
                db.Projects.Add(p);
                db.SaveChanges();

                // Başarılı olursa ana sayfaya dön
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Hata olursa (örneğin resim yüklenemezse) sayfada kal ve hatayı göster
                ViewBag.Error = "Bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        // Proje Silme İşlemi
        public ActionResult Delete(int id)
        {
            // Güvenlik: Sadece admin silebilir
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Silinecek projeyi bul
            var project = db.Projects.Find(id);

            if (project != null)
            {
                // 1. Önce sunucudaki resim dosyasını silelim (Yer kaplamasın)
                // Eğer resim yolu null değilse ve dosya varsa sil
                if (!string.IsNullOrEmpty(project.ImageUrl))
                {
                    string fullPath = Request.MapPath(project.ImageUrl);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // 2. Veritabanından kaydı sil
                db.Projects.Remove(project);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}