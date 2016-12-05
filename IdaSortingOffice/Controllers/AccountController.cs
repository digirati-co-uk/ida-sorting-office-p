using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using IdaSortingOffice.Models;

namespace IdaSortingOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly string Username = ConfigurationManager.AppSettings["username"];
        private readonly string Password = ConfigurationManager.AppSettings["password"];

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Form is not valid; please review and try again.";
                return View("Login");
            }

            if (login.Username == Username && login.Password == Password)
                FormsAuthentication.RedirectFromLoginPage(login.Username, true);

            ViewBag.Error = "Credentials invalid. Please try again.";
            return View("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        
    }
}