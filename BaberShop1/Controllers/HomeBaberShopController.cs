using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BaberShop1.Models;

namespace baberShop.Controllers
{
    public class HomeBaberShopController : Controller

    {
        BARBERSHOPEntities2 _db = new BARBERSHOPEntities2();
        // GET: HomeBaberShop
        public ActionResult Index()
        {
            return View();
        }
    
        public ActionResult getMenu()
        {
            var v = from t in _db.MENUs
                    where t.STATUS_MENU == true
                    select t;
            return PartialView(v.ToList());
        }
     
        public ActionResult getPrice()
        {
            var v = from t in _db.SERVICE_SHOP
                    where t.STATUS_SERVICE == true
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult getContact()
        {
            var v = from t in _db.INFOR_SHOP
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult getContactAddress()
        {
            var v = from t in _db.INFOR_SHOP
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult getEmployee()
        {
            var v = _db.NHANVIENs.Take(4);
            return PartialView(v.ToList());
        }

        public ActionResult getService()
        {
            var v = _db.SERVICE_SHOP.OrderBy(c => c.STATUS_SERVICE == true).Take(4);
            return PartialView(v.ToList());
        }

        public ActionResult getFormBookSeat()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult getFormBookSeat(BOOKING book)
        {
            if (ModelState.IsValid)
            {
                if (Session["ID"] == null)
                {
                    return Redirect("/auth/Login");
                }
                else
                {
                    if (book.NAME_BOOK == null || book.PHONE_BOOK == null || book.TIME_BOOKING == null || book.DATE_BOOKING == null)
                    {
                        Response.Write("<script>alert('Data inserted successfully')</script>");
                        return RedirectToAction("/index");
                    }
                    else
                    {
                        _db.Configuration.ValidateOnSaveEnabled = false;
                        book.ID_USER = Int32.Parse(Session["ID"].ToString());
                        book.TRANGTHAI = 0;
                        _db.BOOKINGs.Add(book);
                        _db.SaveChanges();
                        return RedirectToAction("/profile/"+ Session["USERNAME"]);
                    }
                }
            }
            return View();
        }

        public ActionResult profile()
        {
            if (Session["USERNAME"] != null)
            {
                var id = Int32.Parse(Session["ID"].ToString());

                var profileUser = from t in _db.INFOUSERs
                                  where t.ID_USER == id
                                  select t;

                var bookingList = from t in _db.BOOKINGs
                                    where t.ID_USER == id
                                    select t;
                if (profileUser.Count() == 0)
                {
                    INFOUSER info = new INFOUSER();
                    info.ID_USER = id;
                    _db.INFOUSERs.Add(info);
                    _db.SaveChanges();
                    //ViewBag.ProfileUser = profileUser.FirstOrDefault();
                }
                else
                {
                    ViewBag.ProfileUser = profileUser.FirstOrDefault();
                }
                //Debug.WriteLine("This Profile user:"+ profileUser.FirstOrDefault());
                if (bookingList.Count()==0)
                {
                    return View();
                }
                else
                {
                    ViewBag.BookingList = bookingList.ToList();
                    return View();
                }
            }
            else
            {
                return Redirect("/auth/Login");
            }
        }
        // Handle update information user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult profile(INFOUSER infoUser)
        {
            //đang bỏ vở ở đây
            Int32 idUser = Int32.Parse(Session["ID"].ToString());
            var updateUser = _db.INFOUSERs.Where(x => x.ID_USER == idUser).FirstOrDefault();
            if (updateUser != null)
            {
                updateUser.EMAIL = infoUser.EMAIL;
                updateUser.PHONE = infoUser.PHONE;
                updateUser.ADDRESS_USER = infoUser.ADDRESS_USER;
                updateUser.AVT_USER = infoUser.AVT_USER;
                updateUser.LINK_FB_USER = infoUser.LINK_FB_USER;
                _db.SaveChanges();
            }
            return Redirect(""+Session["USERNAME"]);
        }

        
        public JsonResult cancelBook(int id)
        {
            Boolean result = false;
            var cancelBooking = _db.BOOKINGs.Where(x => x.ID_BOOKING == id).FirstOrDefault();
            if (cancelBooking != null)
            {
                cancelBooking.TRANGTHAI = 1;

                _db.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult EmployeeDetail(int id)
        {
            
            var profileEmployee = from t in _db.NHANVIENs
                                  where t.ID_NHANVIEN == id
                                  select t;
            return View(profileEmployee);
        }

        public ActionResult ServiceDetail(int id)
        {

            var serviceDetail = from t in _db.SERVICE_SHOP
                                  where t.ID_SERVICE == id
                                  select t;
            return View(serviceDetail);
        }

        public ActionResult ListEmployee()
        {
            var v = from t in _db.NHANVIENs
                    select t;
            return PartialView(v.ToList());
        }

        public ActionResult ListService()
        {
            var v = from t in _db.SERVICE_SHOP
                    where t.STATUS_SERVICE == true
                    select t;
            return PartialView(v.ToList());
        }


    }
}