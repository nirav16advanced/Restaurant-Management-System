using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_Management_System.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View("~/Views/User/Index.cshtml");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginModel objUser)
        {
            if (ModelState.IsValid)
            {
                using (SampleDBEntities db = new SampleDBEntities())
                {
                    var obj = db.RestaurantUsers.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.UserID.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        return RedirectToAction("Menu");
                    }
                    else
                    {
                        ViewBag.Message = "UserName or Password is incorrect";
                    }
                }
            }
            return View(objUser);
        }
        [HttpGet]
        public ActionResult Search(string search)
        {
            SampleDBEntities db = new SampleDBEntities();
            return View(db.Menus.Where(x=>x.Name.Contains(search) || x.CategoryName.Contains(search) || x.Type.Contains(search) || search == null).ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Menu menu)
        {
            SampleDBEntities db = new SampleDBEntities();
            db.Menus.Add(menu);
            db.SaveChanges();
            return RedirectToAction("Menu");
        }

        public ActionResult Menu()
        {
            if (Session["UserID"] != null)
            {
                SampleDBEntities db = new SampleDBEntities();
                return View(db.Menus.ToList());
                
            }
            else
            {
                ViewBag.Message = "UserName or Password is incorrect";
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult Edit(int itemid)
        {
            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ItemID == itemid);
            return View(m);
        }

        [HttpPost]

        public ActionResult Edit(Menu menu)
        {
            if(ModelState.IsValid)
            {
                SampleDBEntities db = new SampleDBEntities();
                db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Menu");
            }
            return View(menu);
        }

        [HttpGet]
        public ActionResult Delete(int itemid)
        {
            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ItemID == itemid);
            return View(m);

        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int itemid)
        {
            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ItemID == itemid);
            db.Menus.Remove(m);
            db.SaveChanges();

            return RedirectToAction("Menu");
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Login");
        }
       
        
    }
}