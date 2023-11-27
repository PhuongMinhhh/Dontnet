using BaberShop1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BaberShop1.Areas.Admin.Controllers
{
    
    public class HomeController : Controller
    {

        BARBERSHOPEntities2 _db = new BARBERSHOPEntities2();
        // GET: Admin/Home

        public ActionResult User()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var v = from t in _db.ACCOUNT_USER
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult Index()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var NumberUser = from t in _db.ACCOUNT_USER
                    select t;
            ViewBag.NumberUser = NumberUser.Count();

            var NumberService = from t in _db.SERVICE_SHOP
                             select t;
            ViewBag.NumberService = NumberService.Count();

            var NumberBooking = from t in _db.BOOKINGs
                                select t;
            ViewBag.NumberBooking = NumberBooking.Count();

            var NumberNhanVien = from t in _db.NHANVIENs
                                select t;
            ViewBag.NumberNV = NumberNhanVien.Count();

            var NumberMenu = from t in _db.MENUs
                                 select t;
            ViewBag.NumberMenu = NumberMenu.Count();
            return PartialView();
        }
        public ActionResult Service()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var v = from t in _db.SERVICE_SHOP
                    select t;
            return PartialView(v.ToList());
        }
        public ActionResult Booking()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var v = from t in _db.BOOKINGs
                    select t;
            var id = from t in _db.ACCOUNT_USER
                    select t;
            ViewBag.listIDUser = new SelectList(id, "ID_USER", "USERNAME");
            return PartialView(v.ToList());
        }
        public ActionResult Menu()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var v = from t in _db.MENUs
                    select t;
            return PartialView(v.ToList());
        }
        public ActionResult Employee()
        {
            if (Session["ID"] == null)
            {
                return Redirect("/auth/Login");
            }
            var v = from t in _db.NHANVIENs
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult AddUser()
        {

            return PartialView();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(ACCOUNT_USER accountUser)
        {

            if (ModelState.IsValid)
            {
                var check = _db.ACCOUNT_USER.FirstOrDefault(s => s.USERNAME == accountUser.USERNAME);
                if (accountUser.USERNAME == null || accountUser.PASSWORD_USER == null)
                {
                    ViewBag.error = "Vui lòng điền đầy đủ thông tin!!";
                    return View();
                }
                if (check == null)
                {
                    accountUser.PASSWORD_USER = GetMD5(accountUser.PASSWORD_USER);
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.ACCOUNT_USER.Add(accountUser);
                    _db.SaveChanges();
                    return RedirectToAction("/User");
                }
                else
                {

                    ViewBag.error = "Username đã tồn tại!!!";

                    return View();
                }


            }
            return View();
        }

        public ActionResult EditUser(int id)
        {
            var CurrentUser = _db.ACCOUNT_USER.Where(x => x.ID_USER == id).FirstOrDefault();
            return PartialView(CurrentUser);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ACCOUNT_USER userEdit, int id)
        {
            
            var updateUser = _db.ACCOUNT_USER.Where(x => x.ID_USER == id).FirstOrDefault();
            if (updateUser != null)
            {
               
                updateUser.USERNAME = userEdit.USERNAME;
                updateUser.PASSWORD_USER = GetMD5(userEdit.PASSWORD_USER);
                updateUser.STATUS_ACCOUNT = userEdit.STATUS_ACCOUNT;
                updateUser.CHECK_EMPLOYEE = userEdit.CHECK_EMPLOYEE;
                updateUser.CHECK_ADMIN = userEdit.CHECK_ADMIN;
                _db.SaveChanges();
                return RedirectToAction("/User");
            }

            return PartialView();

        }

        public JsonResult DeleteUser(int id)
        {
            Boolean result = false;
            var obj = _db.ACCOUNT_USER.Find(id);
            var userBook = _db.BOOKINGs.Find(id);
            var userInfo = _db.INFOUSERs.Find(id);

            if (obj != null && userBook != null && userInfo != null)
            {
                _db.ACCOUNT_USER.Remove(obj);
                _db.SaveChanges();
                result = true;
            }else
            {
                result = false;
            }
            return Json(result);
        }
        public ActionResult AddService()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddService(SERVICE_SHOP service_shop, HttpPostedFileBase img)
        {
            var fileName = "";
            var path = "";

            if (ModelState.IsValid)
            {
                if(img != null)
                {
                    fileName = img.FileName;
                    path = Path.Combine(Server.MapPath("~/Content/Upload/img/Service"), fileName);
                    img.SaveAs(path);
                    service_shop.IMG = fileName;
                }
                else
                {
                    service_shop.IMG = "Logo.png";
                }
                var check = _db.SERVICE_SHOP.FirstOrDefault(s => s.ID_SERVICE == service_shop.ID_SERVICE);
                if (service_shop.NAME_SERVICE == null || service_shop.PRICE == null || service_shop.IMG == null || service_shop.DESCRIPSTION == null)
                {
                    ViewBag.error = "Vui lòng điền đầy đủ thông tin!!";
                    return View();
                }
                if (check == null)
                {

                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.SERVICE_SHOP.Add(service_shop);
                    _db.SaveChanges();
                    return RedirectToAction("/Service");
                }
                else
                {

                    ViewBag.error = "Dịch vụ đã tồn tại!!!";
                    return View();
                }


            }
            return View();
        }



        public ActionResult EditService(int id)
        {
            var CurrentService = _db.SERVICE_SHOP.Where(x => x.ID_SERVICE == id).FirstOrDefault();
            return PartialView(CurrentService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditService(SERVICE_SHOP serviceEdit, int id, HttpPostedFileBase img)
        {
            var fileName = "";
            var path = "";
            var updateService = _db.SERVICE_SHOP.Where(x => x.ID_SERVICE == id).FirstOrDefault();

            if (updateService != null)
            {
                if (img != null)
                {
                    fileName = img.FileName;
                    path = Path.Combine(Server.MapPath("~/Content/Upload/img/Service"), fileName);
                    img.SaveAs(path);

                    updateService.NAME_SERVICE = serviceEdit.NAME_SERVICE;
                    updateService.PRICE = (serviceEdit.PRICE);
                    updateService.IMG = fileName;
                    updateService.DESCRIPSTION = serviceEdit.DESCRIPSTION;
                    updateService.STATUS_SERVICE = serviceEdit.STATUS_SERVICE;
                    _db.SaveChanges();
                    return RedirectToAction("/Service");
                }
                else
                {
                    updateService.NAME_SERVICE = serviceEdit.NAME_SERVICE;
                    updateService.PRICE = (serviceEdit.PRICE);
                    updateService.DESCRIPSTION = serviceEdit.DESCRIPSTION;
                    updateService.STATUS_SERVICE = serviceEdit.STATUS_SERVICE;
                    _db.SaveChanges();
                    return RedirectToAction("/Service");
                }
                
            }

            return PartialView();

        }

        public JsonResult DeleteService(int id)
        {
            Boolean result = false;
            var obj = _db.SERVICE_SHOP.Find(id);
            if (obj != null)
            {
                _db.SERVICE_SHOP.Remove(obj);
                _db.SaveChanges();
                result = true;
            }
            return Json(result);
        }


        public ActionResult AddBooking()
        {
            var v = from t in _db.ACCOUNT_USER
                    select t;
            ViewBag.listIDUser = new SelectList(v,"ID_USER","USERNAME");

           

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddBooking(BOOKING book)
        {
            if (ModelState.IsValid)
            {
                
                    if (book.ID_USER == null || book.NAME_BOOK == null || book.PHONE_BOOK == null || book.TIME_BOOKING == null || book.DATE_BOOKING == null)
                    {
                        Response.Write("<script>alert('Data inserted successfully')</script>");
                        return View();
                    }
                    else
                    {
                        _db.Configuration.ValidateOnSaveEnabled = false;
                        book.TRANGTHAI = 0;
                        _db.BOOKINGs.Add(book);
                        _db.SaveChanges();
                        return RedirectToAction("/Booking");
                    }
                
            }
            return View();
        }

        public ActionResult EditBooking(int id)
        {

            var CurrentBook = _db.BOOKINGs.Where(x => x.ID_BOOKING == id).FirstOrDefault();
            var v = from t in _db.ACCOUNT_USER
                    select t;
            DateTimeFormatInfo myDateTimeFormat = new DateTimeFormatInfo();
            myDateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            ViewBag.date = CurrentBook.DATE_BOOKING.Value.GetDateTimeFormats('u')[0].Substring(0, 10);





           
            ViewBag.Status = new SelectList(new List<SelectListItem>
        {
            new SelectListItem {Text = "Thành công", Value = "0"},
            new SelectListItem {Text = "Đang xác nhận", Value = "1"},
            new SelectListItem {Text = "xác nhận hủy", Value = "2"},
        }, "Value", "Text");
            

            return PartialView(CurrentBook);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditBooking(BOOKING booking, int id)
        {
            var dateUpdate = Request.Form["DateBooking"];
            var updateBooking = _db.BOOKINGs.Where(x => x.ID_BOOKING == id).FirstOrDefault();
            DateTime myDate = Convert.ToDateTime(dateUpdate);
            if (updateBooking != null)
            {
                updateBooking.NAME_BOOK = booking.NAME_BOOK;
                updateBooking.PHONE_BOOK = (booking.PHONE_BOOK);
                updateBooking.DATE_BOOKING = myDate;
                updateBooking.TIME_BOOKING = booking.TIME_BOOKING;
                updateBooking.COMMENT = booking.COMMENT;
                updateBooking.TRANGTHAI = booking.TRANGTHAI;
                _db.SaveChanges();
                return RedirectToAction("/Booking");
    
                
            }
            return View();
        }

        public JsonResult DeleteBooking(int id)
        {
            Boolean result = false;
            var obj = _db.BOOKINGs.Find(id);
            if (obj != null)
            {
                _db.BOOKINGs.Remove(obj);
                _db.SaveChanges();
                result = true;
            }
            return Json(result);
        }

        public ActionResult AddMenu()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddMenu(MENU newMenu)
        {
            if (ModelState.IsValid)
            {
                if (newMenu.NAME_MENU == null || newMenu.STATUS_MENU == null)
                {
                    ViewBag.error = "Vui lòng điền đầy đủ thông tin!!";
                    return View();
                }
                else
                {
                    DateTime dateTime = DateTime.UtcNow.Date;
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    newMenu.DATETIME_MENU = dateTime;
                    _db.MENUs.Add(newMenu);
                    _db.SaveChanges();
                    return RedirectToAction("/Menu");
                }
                

            }
            return View();
        }

        public ActionResult EditMenu(int id)
        {
            var CurrentMenu = _db.MENUs.Where(x => x.ID_MENU == id).FirstOrDefault();
            DateTimeFormatInfo myDateTimeFormat = new DateTimeFormatInfo();
            myDateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            ViewBag.date = CurrentMenu.DATETIME_MENU.Value.GetDateTimeFormats('u')[0].Substring(0, 10);
            return PartialView(CurrentMenu);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditMenu(MENU menuCurrent, int id)
        {
            var dateUpdate = Request.Form["DateBooking"];
            var updateMenu = _db.MENUs.Where(x => x.ID_MENU == id).FirstOrDefault();
            DateTime myDate = Convert.ToDateTime(dateUpdate);
            if (updateMenu != null)
            {
                updateMenu.NAME_MENU = menuCurrent.NAME_MENU;
                updateMenu.DATETIME_MENU = myDate;
                updateMenu.STATUS_MENU = menuCurrent.STATUS_MENU;
               _db.SaveChanges();
                return RedirectToAction("/Menu");


            }
            return View();
        }

        public JsonResult DeleteMenu(int id)
        {
            Boolean result = false;
            var obj = _db.MENUs.Find(id);
            if (obj != null)
            {
                _db.MENUs.Remove(obj);
                _db.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult AddEmployee()
        {
            ViewBag.sex = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Text = "Nam", Value = "Nam"},
                    new SelectListItem {Text = "Nữ", Value = "Nữ"},
                }, "Value", "Text");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddEmployee(NHANVIEN newNhanVien, HttpPostedFileBase img)
        {
            var fileName = "";
            var path = "";

            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    fileName = img.FileName;
                    path = Path.Combine(Server.MapPath("~/Content/Upload/img/Employee"), fileName);
                    img.SaveAs(path);
                    newNhanVien.AVT = fileName;
                }
                else
                {
                    newNhanVien.AVT = "Logo.png";
                }
                var check = _db.NHANVIENs.FirstOrDefault(s => s.ID_NHANVIEN == newNhanVien.ID_NHANVIEN);
                if (newNhanVien.NAME_NHANVIEN == null || newNhanVien.SEX == null || newNhanVien.SKILL == null || newNhanVien.AVT == null || newNhanVien.PHONE == null || newNhanVien.LINK_FB == null || newNhanVien.LINK_INSTAGRAM == null || newNhanVien.DESCRIPSTION_NV == null)
                {
                    ViewBag.error = "Vui lòng điền đầy đủ thông tin!!";
                    return View();
                }
                if (check == null)
                {

                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.NHANVIENs.Add(newNhanVien);
                    _db.SaveChanges();
                    return RedirectToAction("/Employee");
                }
                else
                {

                    ViewBag.error = "Nhân Viên đã tồn tại!!!";
                    return View();
                }


            }
            return View();
        }

        public ActionResult EditEmployee( int id)
        {
            var CurrentEmployee = _db.NHANVIENs.Where(x => x.ID_NHANVIEN == id).FirstOrDefault();

            ViewBag.sex = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Text = "Nam", Value = "Nam"},
                    new SelectListItem {Text = "Nữ", Value = "Nữ"},
                }, "Value", "Text");

            return PartialView(CurrentEmployee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditEmployee(NHANVIEN editNhanVien,int id, HttpPostedFileBase img)
        {
            var fileName = "";
            var path = "";
            var updateEmployee = _db.NHANVIENs.Where(x => x.ID_NHANVIEN == id).FirstOrDefault();

            if (updateEmployee != null)
            {
                if (img != null)
                {
                    fileName = img.FileName;
                    path = Path.Combine(Server.MapPath("~/Content/Upload/img/Employee"), fileName);
                    img.SaveAs(path);

                    updateEmployee.NAME_NHANVIEN = editNhanVien.NAME_NHANVIEN;
                    updateEmployee.SEX = (editNhanVien.SEX);
                    updateEmployee.AVT = fileName;
                    updateEmployee.SKILL = editNhanVien.SKILL;
                    updateEmployee.PHONE = editNhanVien.PHONE;
                    updateEmployee.LINK_FB = editNhanVien.LINK_FB;
                    updateEmployee.LINK_INSTAGRAM = editNhanVien.LINK_INSTAGRAM;
                    updateEmployee.DESCRIPSTION_NV = editNhanVien.DESCRIPSTION_NV;
                    _db.SaveChanges();
                    return RedirectToAction("/Employee");
                }
                else
                {
                    updateEmployee.NAME_NHANVIEN = editNhanVien.NAME_NHANVIEN;
                    updateEmployee.SEX = (editNhanVien.SEX);
                    updateEmployee.SKILL = editNhanVien.SKILL;
                    updateEmployee.PHONE = editNhanVien.PHONE;
                    updateEmployee.LINK_FB = editNhanVien.LINK_FB;
                    updateEmployee.LINK_INSTAGRAM = editNhanVien.LINK_INSTAGRAM;
                    updateEmployee.DESCRIPSTION_NV = editNhanVien.DESCRIPSTION_NV;
                    _db.SaveChanges();
                    return RedirectToAction("/Employee");
                }

            }

            return PartialView();
        }

        public JsonResult DeleteEmployee(int id)
        {
            Boolean result = false;
            var obj = _db.NHANVIENs.Find(id);
            if (obj != null)
            {
                _db.NHANVIENs.Remove(obj);
                _db.SaveChanges();
                result = true;
            }
            return Json(result);
        }
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