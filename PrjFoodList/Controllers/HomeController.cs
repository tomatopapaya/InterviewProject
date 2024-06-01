﻿using PrjFoodList.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Web;
using System.Web.Security;
using System.Diagnostics;

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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string fTitle, string fAddress, DateTime fDate, HttpPostedFileBase photo)
        {
            food resturant = new food(); //food是表格
            resturant.fTitle = fTitle;
            resturant.fAddress = fAddress;
            resturant.fDate = fDate;
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

            return View(resturant);
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string fUser, string fPwd)
        {
            var member = db.Member.Where(m => m.fUser == fUser && m.fPwd == fPwd).FirstOrDefault();
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            Session["Welcome"] = member.fName + "歡迎光臨";

            FormsAuthentication.RedirectFromLoginPage(fUser, true);
            return RedirectToAction("Index", "Member");
        }

        
    }
}