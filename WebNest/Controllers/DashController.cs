using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNest.Data;

namespace WebNest.Controllers
{
    public class DashController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public DashController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult MyWebsite()
        {
            int loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId") ?? 0;

            // Fetch companies belonging to this user
            // Adjust the filter based on how you link Registration to a user
            var companies = dbContext.Registration
                 .Where(r => r.LoggedInUserId == loggedInUserId)
                 .ToList();  // filter by userId if you have that column

            ViewBag.Companies = companies;
            ViewBag.CompanyCount = companies.Count;

            return View();
        }



        [HttpPost]
        public IActionResult UpdateCompany(WebNest.Models.Registration model)
        {
            var existing = dbContext.Registration.Find(model.RegId);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Company not found!";
                return RedirectToAction("MyWebsite");
            }

            existing.ClientName = model.ClientName;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Address = model.Address;
            existing.City = model.City;
            existing.CompanyName = model.CompanyName;
            existing.CompanyType = model.CompanyType;
            existing.Employees = model.Employees;
            existing.UserId = model.UserId;

            dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Company updated successfully!";
            return RedirectToAction("MyWebsite");
        }

        [HttpPost]
        public IActionResult DeleteCompany(int id)
        {
            int loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId") ?? 0;

            var company = dbContext.Registration
                .FirstOrDefault(r => r.RegId == id && r.LoggedInUserId == loggedInUserId);

            if (company != null)
            {
                dbContext.Registration.Remove(company);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Company deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Company not found or access denied!";
            }

            return RedirectToAction("MyWebsite");
        }




    }


}
