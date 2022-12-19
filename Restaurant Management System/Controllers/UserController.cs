using NLog;
using Restaurant_Management_System.Common;
using Restaurant_Management_System.DAL;
using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_Management_System.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View("~/Views/User/Index.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Create(RestaurantUser user)
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
                    
                    db.RestaurantUsers.Add(user);
                    await db.SaveChangesAsync();
                    return View();
                }
                catch (Exception dbEx)
                {
                    logger.Error(dbEx, "Usercontroller Create Method");
                    ViewBag.ErrorMessage = dbEx.Message;
                    return View();
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
        public JsonResult doesUserNameExist(string UserName)
        {
            SampleDBEntities db = new SampleDBEntities();
            return Json(!db.RestaurantUsers.Any(x => x.USERNAME == UserName), JsonRequestBehavior.AllowGet);
        }
    }
}