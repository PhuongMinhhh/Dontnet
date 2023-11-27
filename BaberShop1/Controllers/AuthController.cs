using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using BaberShop1.Models;
using System.Web.UI;
using System.Text;
using System.Data.Entity;

namespace baberShop.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        BARBERSHOPEntities2 _db = new BARBERSHOPEntities2();
        public ActionResult Login()
        {
            if (Session["USERNAME"]!=null)
            {
               return Redirect("/");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ACCOUNT_USER _user)
        {
            if (ModelState.IsValid)
            {
                if(_user.USERNAME == null || _user.PASSWORD_USER ==null) {
                    ViewBag.error = "Vui lòng điền username hoặc password!";
                    return View();
                }

                var f_password = GetMD5(_user.PASSWORD_USER);
                var data = _db.ACCOUNT_USER.Where(s => s.USERNAME.Equals(_user.USERNAME) && s.PASSWORD_USER.Equals(f_password)).ToList();
                
                if (data.Count() > 0)
                {
                    //add session
                    

                    Session["USERNAME"] = data.FirstOrDefault().USERNAME ;
                    Session["ID"] = data.FirstOrDefault().ID_USER;
                    Session["CHECK_EMPLOYEE"] = data.FirstOrDefault().CHECK_EMPLOYEE;
                    Session["CHECK_ADMIN"] = data.FirstOrDefault().CHECK_ADMIN;
                    return Redirect("/HomeBaberShop/Index");
                }
                else
                {
                    ViewBag.error = "Đã xảy ra lỗi username hoặc password!";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ACCOUNT_USER _user)
        {
            if (ModelState.IsValid)
            {
                var check = _db.ACCOUNT_USER.FirstOrDefault(s => s.USERNAME == _user.USERNAME);
                if(_user.USERNAME == null || _user.PASSWORD_USER == null)
                {
                    ViewBag.error = "Vui lòng điền đầy đủ thông tin!!";
                    return View();
                }
                if (check == null)
                {
                    _user.PASSWORD_USER = GetMD5(_user.PASSWORD_USER);
                    _user.CHECK_ADMIN = false;
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.ACCOUNT_USER.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("/login");
                }
                else
                {
                    ViewBag.error = "Username đã tồn tại!!!";
                    return View();
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return Redirect("/HomeBaberShop/Index");
        }

        //fogot view
        public ActionResult Forgot()
        {
            return View();
        }
        //handle check username forget
        [HttpPost]
        public ActionResult Forgot(ACCOUNT_USER _user)
        {
            if (ModelState.IsValid)
            {
                var data = _db.ACCOUNT_USER.Where(s => s.USERNAME.Equals(_user.USERNAME));
                var check = _db.ACCOUNT_USER.FirstOrDefault(s => s.USERNAME == _user.USERNAME);
                if (_user.USERNAME == null)
                {
                    ViewBag.error = "Vui lòng điền đúng username của bạn!!";
                    return View();
                }
                if (check != null)
                {
                    //ViewBag.error = data.FirstOrDefault().ID_USER;
                    return RedirectToAction("/newPassword/"+ data.FirstOrDefault().ID_USER);
                }
                else
                {

                    ViewBag.error = "Không tìm thấy username của bạn!";

                    return View();
                }
            }
            return View();
        }

        //enter new password
        public ActionResult NewPassword()
        {
            return View();
        }
        //handle new password
        [HttpPost]
        public ActionResult NewPassword(ACCOUNT_USER _user, int id)
        {
            if (ModelState.IsValid)
            {
                var check = _db.ACCOUNT_USER.FirstOrDefault(s => s.ID_USER == id);
                if (check != null)
                {
                    ACCOUNT_USER userUpdate = new ACCOUNT_USER();
                    userUpdate = _db.ACCOUNT_USER.Find(id);
                    userUpdate.PASSWORD_USER = GetMD5(_user.PASSWORD_USER);
                    _db.Entry(userUpdate).State = EntityState.Modified;
                    _db.SaveChanges();
                    ViewBag.success = "Cập nhật thành công";
                    //return RedirectToAction("/login");
                }else
                {
                    ViewBag.error = "Không tìm thấy user!!";
                }
                
            }
            return View();
        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}