using PrjFoodList.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;


namespace PrjFoodList.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        foodEntities db = new foodEntities();
        int pagesize = 5;
        public ActionResult Index(string searchString, int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var foods = db.food.OrderBy(m => m.fDate).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                foods = db.food.OrderBy(m => m.fDate).Where(m => m.fTitle.Contains(searchString)).ToList();
            }
            var result = foods.ToPagedList(currentPage, pagesize);
            return View("../Home/Index", "_LayoutMember", result);
        }


    }
}