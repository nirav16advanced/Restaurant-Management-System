using Newtonsoft.Json;
using NLog;
using Restaurant_Management_System.Common;
using Restaurant_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Restaurant_Management_System.Controllers
{
    public class LoginController : Controller
    { 
        // Generating logger object
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();


        // GET: Login
       public ActionResult Login()
        {
            return View();
        }

        // validate login details
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            bool isCapthcaValid = ValidateCaptcha(Request["g-recaptcha-response"]);
            if (ModelState.IsValid)
            {
                if (isCapthcaValid)
                {
                    //Password decryptPassword = new Password();
                    PasswordBase64 decryptPassword = new PasswordBase64();
                    using (SampleDBEntities db = new SampleDBEntities())
                    {
                        var obj = db.RESTAURANTUSERs.Where(a => a.USERNAME.Equals(model.UserName)).FirstOrDefault();
                        if (obj != null && PasswordBase64.DecryptPassword(obj.PASSWORD) == model.Password)
                        {
                            Session["UserID"] = obj.USER_ID.ToString();
                            Session["UserName"] = obj.USERNAME.ToString();
                            logger.Info("User Login.UserName=" + Session["UserName"]);

                            return RedirectToAction("Menu");
                        }
                        else
                        {
                            ViewBag.Message = "UserName or Password is incorrect";
                        }
                    }
                    return View(model);//some code After success
                }
                else
                {
                    ModelState.AddModelError("", "You have put wrong Captcha,Please ensure the authenticity !!!");
                    ModelState.Remove("Password");

                    //Should load sitekey again
                    return View();
                }
            }
            return View();

        }


        // Return view for register
        public ActionResult Register()
        {
            return View("~/Views/User/Index.cshtml");
        }


        /*[HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginModel objUser)
        {
            if (ModelState.IsValid)
            {
                //Password decryptPassword = new Password();
                PasswordBase64 decryptPassword = new PasswordBase64();
                using (SampleDBEntities db = new SampleDBEntities())
                {
                    var obj = db.RESTAURANTUSERs.Where(a => a.USERNAME.Equals(objUser.UserName)).FirstOrDefault();
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
        }*/


        // Get view for search and method for searching
        [HttpGet]
        public ActionResult Search(string search)
        {
            SampleDBEntities db = new SampleDBEntities();
            return View(db.Menus.Where(x => x.NAME.Contains(search) || x.TYPE.Contains(search) || x.Category.CATEGORY_NAME.Contains(search)|| search == null).ToList());
        }

        // Get view for item create
        [HttpGet]
        public ActionResult Create()
        {
            SelectListItem[] itemName = GetItemList();

            return View();
        }


        // add item method in db
        [HttpPost]
        public ActionResult Create(Menu menu)
        {
            SelectListItem[] itemName = GetItemList();

            try
            {
                SampleDBEntities db = new SampleDBEntities();
                menu.CREATED_BY = Session["UserName"].ToString();
                menu.CREATED_DATE = DateTime.Now;
                // menu.UPDATED_BY = null;
                //menu.UPDATED_DATE = DateTime.Now;
                db.Menus.Add(menu);
                db.SaveChanges();
                logger.Info("New Item Created.UserName=" + Session["UserName"] );
                return RedirectToAction("Menu");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                return View();
               /* Exception raise = dbEx;
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
                throw raise;*/
            }

        }

        // Method for menu
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

        //Get edit view for editing item details
        [HttpGet]
        public ActionResult Edit(int? itemid)
        {
            SelectListItem[] itemName = GetItemList();

            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            //m.UPDATED_BY = Session["UserName"].ToString();
            //m.UPDATED_DATE = DateTime.Now;
            return View(m);
        }

        //Edit method for editing item details
        [HttpPost]
        public ActionResult Edit(Menu menu)
        {
            SelectListItem[] itemName = GetItemList();


            if (ModelState.IsValid)
            {
                SampleDBEntities db = new SampleDBEntities();

                menu.UPDATED_BY = Session["UserName"].ToString();
                menu.UPDATED_DATE = DateTime.Now;
                db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                logger.Info("Item Edited.UserName=" + Session["UserName"]);
                return RedirectToAction("Menu");
            }
            return View(menu);
        }

        // Get view for delete record
        [HttpGet]
        public ActionResult Delete(int? itemid)
        {
            SelectListItem[] itemName = GetItemList();

            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            return View(m);

        }

        //Method for deleting record
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int itemid)
        {
            SampleDBEntities db = new SampleDBEntities();
            Menu m = db.Menus.Single(x => x.ITEM_ID == itemid);
            db.Menus.Remove(m);
            db.SaveChanges();
            logger.Info("Item Deleted.UserName=" + Session["UserName"]);

            return RedirectToAction("Menu");
        }


        //method for logout
        public ActionResult Logout()
        {
            Session["UserId"] = null;
            logger.Info("User Logout.UserName=" + Session["UserName"]);

            return RedirectToAction("Login");
        }


        //method for uniq item register 
        public JsonResult doesItemExist(string itemName)
        {
            SampleDBEntities db = new SampleDBEntities();
            return Json(!db.RESTAURANTUSERs.Any(x => x.USERNAME == itemName), JsonRequestBehavior.AllowGet);
        }


        //for generating dropdown
        protected SelectListItem[] GetItemList()
        {
            SampleDBEntities db = new SampleDBEntities();

            var names = db.Categories.Select(n => new SelectListItem()
            {
                Text = n.CATEGORY_NAME,
                Value = n.CATEGORY_ID.ToString()
            }).ToList();

            SelectListItem[] itemName = names.ToArray();

            return ViewBag.iname = itemName.Select(n => new SelectListItem()
            {
                Text = n.Text,
                Value = n.Value.ToString()
            }).ToArray(); ; // this will carry data to the view page

        }

       /* [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();

        }*/


        
        // validate captcha
        [AllowAnonymous]
        public bool ValidateCaptcha(string response)
        {
            //  Setting _Setting = repositorySetting.GetSetting;

            //secret that was generated in key value pair  
            string secret = ConfigurationManager.AppSettings["GoogleSecretkey"]; //WebConfigurationManager.AppSettings["recaptchaPrivateKey"];

            var client = new WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            return Convert.ToBoolean(captchaResponse.Success);

        }


        // method send email
        public void SendEmail(RESTAURANTUSER admin)
        {
            SampleDBEntities db = new SampleDBEntities();
            //var admin = db.RESTAURANTUSERs.Where(a => a.USERNAME == username).FirstOrDefault();



            // if condition is pending for verification

            var fromEmail = new MailAddress("autodidact.project4@gmail.com", "Roof Top Support Center");
            var toEmail = new MailAddress(admin.EMAIL);



            var fromEmailPassword = "tlyenzpidtaefhui";



            string subject = "Password Recovery";

            string body = "<br/> Hello " + admin.USERNAME +
                            "<br/><br/> We have successfuly processed your request for forget password." +
                            "<br/><br/> Your <br/> Admin-Id : " + admin.USER_ID +
                            "<br/>      Password : " + PasswordBase64.DecryptPassword(admin.PASSWORD) +
                            "<br/><br/> You can access now with this password! :)" +
                            "<br/><br/> Regards" +
                            "<br/> Restaurant Management System";



            var smtpRequest = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };



            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtpRequest.Send(message);
        }

        // get view for forgot password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        
        //method for forget password
        [HttpPost]
        public ActionResult ForgotPassword(string userName)
        {
            SampleDBEntities db = new SampleDBEntities();
            var admin = db.RESTAURANTUSERs.Where(a => a.USERNAME == userName).FirstOrDefault();



            if (admin != null && admin.USERNAME == userName)
            {
                SendEmail(admin);
            }
            else
            {
                ViewBag.Message = "Details mismatch. Please enter valid details else register to access the portal.";
                return View();
            }



            return RedirectToAction("Login", "Login");
        }



    }
}