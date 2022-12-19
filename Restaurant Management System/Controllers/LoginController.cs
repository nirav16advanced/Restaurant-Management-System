using NLog;
using Restaurant_Management_System.Common;
using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Restaurant_Management_System.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
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
                //Password decryptPassword = new Password();
                PasswordBase64 decryptPassword = new PasswordBase64();
                using (SampleDBEntities db = new SampleDBEntities())
                {
                    var obj = db.RestaurantUsers.Where(a => a.USERNAME.Equals(objUser.UserName)).FirstOrDefault();
                    if (obj != null && PasswordBase64.DecryptPassword(obj.PASSWORD) == objUser.Password)
                    {
                        Session["UserID"] = obj.USER_ID.ToString();
                        Session["UserName"] = obj.USERNAME.ToString();
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
            return View(db.Menus.Where(x=>x.NAME.Contains(search)  || x.TYPE.Contains(search) || search == null).ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            SelectListItem[] courseName = GetCourseList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Menu menu)
        {
            SelectListItem[] courseName = GetCourseList();

            try
            {
                SampleDBEntities db = new SampleDBEntities();
                menu.CREATED_BY = Session["UserName"].ToString();
                menu.CREATED_DATE = DateTime.Now;
               // menu.UPDATED_BY = null;
                //menu.UPDATED_DATE = DateTime.Now;
                db.Menus.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Menu");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

        }

        public ActionResult Menu()
        {
            if (Session["UserID"] != null)
            {
                SampleDBEntities db = new SampleDBEntities();

                var menu = db.Menus.Include(m => m.Category).ToList();
                return View(menu);
                
            }
            else
            {
                ViewBag.Message = "UserName or Password is incorrect";
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult Edit(int? itemid)
        {
            SelectListItem[] courseName = GetCourseList();

            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            //m.UPDATED_BY = Session["UserName"].ToString();
            //m.UPDATED_DATE = DateTime.Now;
            return View(m);
        }

        [HttpPost]

        public ActionResult Edit(Menu menu)
        {

            if (ModelState.IsValid)
            {
                SampleDBEntities db = new SampleDBEntities();
                
                menu.UPDATED_BY = Session["UserName"].ToString();
                menu.UPDATED_DATE = DateTime.Now;
                db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Menu");
            }
            return View(menu);
        }

        [HttpGet]
        public ActionResult Delete(int? itemid)
        {
            SelectListItem[] courseName = GetCourseList();

            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            return View(m);

        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int itemid)
        {
            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            db.Menus.Remove(m);
            db.SaveChanges();

            return RedirectToAction("Menu");
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Login");
        }

        protected SelectListItem[] GetCourseList()
        {
            SampleDBEntities db = new SampleDBEntities();

            var names = db.Categories.Select(n => new SelectListItem()
            {
                Text = n.CATEGORY_NAME,
                Value = n.CATEGORY_ID.ToString()
            }).ToList();

            SelectListItem[] courseName = names.ToArray();

            return ViewBag.cname = courseName.Select(n => new SelectListItem()
            {
                Text = n.Text,
                Value = n.Value.ToString()
            }).ToArray(); ; // this will carry data to the view page

        }


    }
}