using Restaurant_Management_System.DAL;
using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_Management_System.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View("~/Views/User/Index.cshtml");
        }
        public ActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                RegisterDataLayer dal = new RegisterDataLayer();
                string message = dal.SignUpUser(model);
            }
            else
            {
                return View("~/Views/User/Index.cshtml");
            }
            return View();
        }
    }
}