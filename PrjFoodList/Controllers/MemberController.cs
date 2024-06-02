using PrjFoodList.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using System.Diagnostics;
using System.Web;

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

            foods = foods.Where(m => m.UserID == User.Identity.Name).ToList();
            var result = foods.ToPagedList(currentPage, pagesize);
            return View("../Home/Index", "_LayoutMember", result);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string fTitle, string fAddress, DateTime fDate, HttpPostedFileBase photo, string ftype, string UserID)
        {
            food resturant = new food(); //food是表格
            resturant.fTitle = fTitle;
            resturant.fAddress = fAddress;
            resturant.fDate = fDate;
            resturant.ftype = ftype;
            resturant.UserID = UserID;
            //檔案上傳
            if (photo != null)
            {
                if (ModelState.IsValid)
                {
                    if (photo != null)
                    {

                        resturant.fImg = new byte[photo.ContentLength];
                        photo.InputStream.Read(resturant.fImg, 0, photo.ContentLength);
                    }
                }

                db.food.Add(resturant);
                db.SaveChanges();
                Debug.WriteLine("i get it");
                return RedirectToAction("Index");
            }
            else
            {
                db.food.Add(resturant);
                db.SaveChanges();
            }

            return View(resturant);
        }

        public ActionResult Delete(int id)
        {
            var restaurant = db.food.Where(m => m.fId == id).FirstOrDefault();
            db.food.Remove(restaurant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var resturant = db.food.Where(m => m.fId == id).FirstOrDefault();
            return View(resturant);
        }

        [HttpPost]
        public ActionResult Edit(string fTitle, string fAddress, DateTime fDate, HttpPostedFileBase photo, string ftype, string UserID)
        {
            var resturant = db.food.Where(m => m.fTitle == fTitle).FirstOrDefault();
            resturant.fTitle = fTitle;
            resturant.fAddress = fAddress;
            resturant.fDate = fDate;
            resturant.ftype = ftype;
            resturant.UserID = UserID;
            //檔案上傳
            if (photo != null)
            {
                if (ModelState.IsValid)
                {
                    if (photo != null)
                    {

                        resturant.fImg = new byte[photo.ContentLength];
                        photo.InputStream.Read(resturant.fImg, 0, photo.ContentLength);
                    }
                }
              
            }

            db.SaveChanges();
            return RedirectToAction("Index");
            
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
    }
}