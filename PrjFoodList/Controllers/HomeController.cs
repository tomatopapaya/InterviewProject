using PrjFoodList.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace PrjFoodList.Controllers
{
    public class HomeController : Controller
    {
        foodEntities db = new foodEntities();
        int pagesize = 5;

        public ActionResult Index(string searchString, int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var foods = db.food.OrderBy(m => m.fDate).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                foods = db.food.OrderBy(m => m.fDate).Where(s => s.fTitle.Contains(searchString)).ToList();
            }
            var result = foods.ToPagedList(currentPage, pagesize);
            return View(result);
        }

        public FileContentResult GetImage(int fId)

        {
            food requestedPhoto = db.food.FirstOrDefault(m => m.fId == fId);

            if (requestedPhoto != null)
            {
                return File(requestedPhoto.fImg, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Member Member)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }
            var member = db.Member
                .Where(m => m.fUser == Member.fUser)
                .FirstOrDefault();

            if (member == null)
            {
                db.Member.Add(Member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Message = "此帳號已有人使用，註冊失敗";

            return View();
        }
    }
}