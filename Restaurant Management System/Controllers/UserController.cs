using NLog;
using Restaurant_Management_System.Common;
using Restaurant_Management_System.DAL;
using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace Restaurant_Management_System.Controllers
{
    public class UserController : Controller
    {
        //logger for logging file
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //get view for register
        public ActionResult Register()
        {
            return View("~/Views/User/Index.cshtml");
        }

        //Method for create user
        [HttpPost]
        public async Task<ActionResult> Create(RESTAURANTUSER user)
        {


            if (ModelState.IsValid)
            {

                PasswordBase64 encryptPassword = new PasswordBase64();

                SampleDBEntities db = new SampleDBEntities();

                try
                {
                    user.CREATED_BY = user.USERNAME;
                    user.CREATED_DATE = DateTime.Now;
                    user.PASSWORD = PasswordBase64.EncryptPassword(user.PASSWORD);

                    db.RESTAURANTUSERs.Add(user);
                    await db.SaveChangesAsync();
                    logger.Info("New User Register.UserName=" + Session["UserName"]);


                    return View();

                }
                catch (Exception dbEx)
                {
                    logger.Error(dbEx, "Usercontroller Create Method");
                    ViewBag.ErrorMessage = dbEx.Message;

                }

            }
            return RedirectToAction("Index");
        }

        /* public ActionResult Create(UserModel model)
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
         } */


        //json for validiting uniq username
        public JsonResult doesUserNameExist(string UserName)
        {
            SampleDBEntities db = new SampleDBEntities();
            return Json(!db.RESTAURANTUSERs.Any(x => x.USERNAME == UserName), JsonRequestBehavior.AllowGet);
        }


        
    }
}
        