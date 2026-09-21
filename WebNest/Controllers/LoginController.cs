using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using WebNest.Data;
using WebNest.Models;

namespace WebNest.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext dbContext;
            public LoginController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        // GET: Signup
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(Signup model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = dbContext.Signup.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already registered!");
                    return View(model);
                }

                // Hash the password before saving
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                dbContext.Signup.Add(model);
                dbContext.SaveChanges();

                TempData["Success"] = "Account created successfully! Please login.";
               // Redirect to Login page
            }
            return RedirectToAction("Login", "Login");
        }






        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AdminLogin()
        {
            if (!dbContext.Admin.Any())
            {
                dbContext.Admin.Add(new Admin
                {
                    AdminId = "admin123",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123")
                });
                dbContext.SaveChanges();
            }

            return View();
        }



        [HttpPost]
        public IActionResult AdminLogin(Admin model)
        {
            var admin = dbContext.Admin.FirstOrDefault(a => a.AdminId == model.AdminId);

            if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            {
                // ✅ Redirect directly to AdminDash in RegistrationController
                return RedirectToAction("CompDetails", "Registration");
            }

            // ✅ Use TempData so SweetAlert can pick it up
            TempData["AdminError"] = "Invalid Admin ID or Password. Please try again.";
            return RedirectToAction("AdminLogin");
        }





        [HttpPost]
        public IActionResult Login(string Email, string password)
        {
            // CHANGE: Use Signup table instead of Registration
            var user = dbContext.Signup.FirstOrDefault(u => u.Email == Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                HttpContext.Session.SetInt32("LoggedInUserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
                

                return RedirectToAction("MyWebsite", "Dash");
            }

            TempData["Error"] = "Invalid Email or Password.";
            return RedirectToAction("Login");
        }





    }
}
