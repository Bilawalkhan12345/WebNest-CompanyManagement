using Microsoft.AspNetCore.Mvc;
using WebNest.Data;
using WebNest.Models;

namespace WebNest.Controllers
{
    public class RegistrationController : Controller
    {

        private readonly ApplicationDbContext DbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RegistrationController(ApplicationDbContext DbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.DbContext = DbContext;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult AdminDash()
        {
            var users = DbContext.Signup.ToList();
            var companies = DbContext.Registration.ToList();

            ViewBag.Users = users;
            ViewBag.Companies = companies;

            return View();
        }

        // 1. SHOW the page — gets all users from DB
        public IActionResult CompDetails()
        {
            var users = DbContext.Signup.ToList();
            var companies = DbContext.Registration.ToList();

            ViewBag.Users = users;
            ViewBag.Companies = companies;

            return View();
        }






        [HttpPost]
        public async Task<IActionResult> Add(Registration model)
        {

            if (!ModelState.IsValid)
                return View("SignUp", model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View("SignUp", model);
            }

            int loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId") ?? 0;

            if (loggedInUserId == 0)
            {
                TempData["ErrorMessage"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Login");
            }

            int existingCount = DbContext.Registration.Count(r =>
            r.LoggedInUserId == loggedInUserId);


            if (existingCount >= 5)
            {
                TempData["ErrorMessage"] = "You can only add up to 5 companies.";
                return RedirectToAction("AdminDash");
            }


            var user = new Registration
            {
                ClientName = model.ClientName,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                CompanyName = model.CompanyName,
                CompanyType = model.CompanyType,
                Employees = model.Employees,
                UserId = model.UserId,

                LoggedInUserId = loggedInUserId
            };

            await DbContext.Registration.AddAsync(user);
            await DbContext.SaveChangesAsync();


            TempData["SuccessMessage"] = "Company added successfully!";
            return RedirectToAction("MyWebsite", "Dash"); // or return to the same view
        }








        [HttpPost]
        public async Task<IActionResult> Register(Registration model)
        {
            if (model.ImageFile != null)
            {
                string folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);


                string fileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Save file name in database
                model.ProfilePicture = fileName;
            }

            DbContext.Registration.Add(model);
            await DbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }








        // 🔹 UPDATE
        [HttpPost]
        public IActionResult UpdateUser(Registration model)
        {
            var existing = DbContext.Registration.Find(model.RegId);

            if (existing == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("CompDetails");
            }

            // Update fields
            existing.ClientName = model.ClientName;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Address = model.Address;
            existing.City = model.City;
            existing.CompanyName = model.CompanyName;
            existing.CompanyType = model.CompanyType;
            existing.Employees = model.Employees;
            existing.UserId = model.UserId;

            DbContext.SaveChanges();

            TempData["SuccessMessage"] = "Updated successfully!";
            return RedirectToAction("CompDetails");
        }

        // Delete entire user + their companies
        [HttpPost]
        public IActionResult DeleteSignupUser(int id)
        {
            // Delete all companies belonging to this user
            var companies = DbContext.Registration
                .Where(r => r.LoggedInUserId == id).ToList();
            DbContext.Registration.RemoveRange(companies);

            // Delete the user from Signup table
            var user = DbContext.Signup.Find(id);
            if (user != null)
                DbContext.Signup.Remove(user);

            DbContext.SaveChanges();

            TempData["SuccessMessage"] = "User and all their companies deleted!";
            return RedirectToAction("CompDetails");
        }



        [HttpPost]
        public IActionResult DeleteCompany(int id)
        {
            var company = DbContext.Registration.Find(id);
            if (company != null)
            {
                DbContext.Registration.Remove(company);
                DbContext.SaveChanges();
                TempData["SuccessMessage"] = "Company deleted!";
            }
            return RedirectToAction("CompDetails");
        }




    }
}
