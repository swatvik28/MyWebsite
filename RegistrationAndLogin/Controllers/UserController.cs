using RegistrationAndLogin.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;


namespace RegistrationAndLogin.Controllers
{
    public class UserController : Controller
    {
        //Registration 
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Registration Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude="isEmailVerified, ActivationCode")] User user)
        {
            bool Status = false;
            string Message = "";
            

            //Model Validation
            if(ModelState.IsValid)
            {
                #region Email Validation (if exists or not)

                bool isExist = isEmailExists(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("Emailexist", "Email Already exists in the Database.");
                    return View(user);
                }
                #endregion

                #region ActivationCode Generation

                user.ActivationCode = Guid.NewGuid();

                #endregion

                #region PasswordHashing
                user.Password = crypto.Hash(user.Password);
                user.ConfirmPassword = crypto.Hash(user.ConfirmPassword);
                #endregion

                user.isEmailVerified = false;

                #region SaveTo DataBase
                using (RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities())
                {
                    RE.Users.Add(user);
                    RE.SaveChanges();

                    sendVerifyLink(user.EmailID, (user.ActivationCode).ToString());
                    Message = "Registration successfull!! Verification link has been sent to your mail.";
                    Status = true;
                }
                #endregion
            }
            else
            {
                Message = "Invalid Request";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify Account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities())
            {
                RE.Configuration.ValidateOnSaveEnabled = false;
                var acc = RE.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if(acc != null)
                {
                    acc.isEmailVerified = true;
                    RE.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = " Invalid Request.";
                }
                ViewBag.Status = Status;
                return View();
            }
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl)
        {
            string message = "";
            using (RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities())
            {
                var v = RE.Users.Where(a => a.EmailID == login.EmailId).FirstOrDefault();
                if(v != null)
                {
                    if(string.Compare(crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 1;
                        var ticket = new FormsAuthenticationTicket(login.EmailId, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("ListClasses", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid Credintials!!";
                    }
                }
                else
                {
                    message = "Invalid Credintials!!";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        // Logout
         [Authorize]
         public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public static bool isEmailExists(string EmailId)
        {
            using (RegistrationAndLoginEntities RE = new RegistrationAndLoginEntities())
            {
                var v = RE.Users.Where(a => a.EmailID == EmailId).FirstOrDefault();
                return v == null ? false : true;
            }
        }

        [NonAction]
        public void sendVerifyLink(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("manyam.pratyusha@gmail.com", "MyWebsite");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "pratyusai@5"; // Replace with actual password
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
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
            })
                smtp.Send(message);
        }
    }
}