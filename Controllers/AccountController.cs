using myBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq; 


namespace myBlog.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private readonly myBlogPrEntities1 db = new myBlogPrEntities1(); 

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var admin = db.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);

            if (admin != null)
            {
                Session["admin"] = username;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear(); // veya Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

    }
}