using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DTOs;
using ZeroHunger.EF;

namespace ZeroHunger.Controllers
{
    public class UsersController : Controller
    {
        private zerohungerDbEntities db = new zerohungerDbEntities();

        // GET: Users
        [HttpGet]
        public ActionResult Index()
        {
            var db = new zerohungerDbEntities();
            var data = db.Users.ToList();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>();
            });
            var mapper = new Mapper(config);
            var data2 = mapper.Map<List<UserDTO>>(data);
            return View(data2);
        }





        /*Login*/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(UserDTO User)
        {
            var db = new zerohungerDbEntities();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = new Mapper(config);
            var conData = mapper.Map<User>(User);

            var existingUser = db.Users.FirstOrDefault(c => c.Username == conData.Username);





            if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Restaurant")
            {
                // Create cookie so that it can store user information
                HttpCookie UserCookie = new HttpCookie("UserInfo");
                UserCookie["Username"] = existingUser.Username;
                UserCookie["UserId"] = existingUser.UserID.ToString();
                UserCookie.Expires = DateTime.Now.AddMinutes(2); // Set the cookie to expire time
                Response.Cookies.Add(UserCookie);

                return RedirectToAction("Deshboard", "Restaurant");
            }
            else if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Employee")
            {
                HttpCookie EmployeeCookie = new HttpCookie("EmployeeInfo");
                EmployeeCookie["Username"] = existingUser.Username;
                EmployeeCookie["UserId"] = existingUser.UserID.ToString();
                EmployeeCookie.Expires = DateTime.Now.AddMinutes(2); //set time
                Response.Cookies.Add(EmployeeCookie);

                return RedirectToAction("Index", "Employee");

            }
            else if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Admin")
            {
                HttpCookie AdminCookie = new HttpCookie("AdminInfo");
                AdminCookie["Username"] = existingUser.Username;
                AdminCookie["UserId"] = existingUser.UserID.ToString();
                AdminCookie.Expires = DateTime.Now.AddMinutes(2); //set time
                Response.Cookies.Add(AdminCookie);

                return RedirectToAction("Index", "Admin");

            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
        }


        /*login end*/











        // GET: Users/Details/5

        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Username,Password,Role,Name,ContactInfo,Location")] UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var db = new zerohungerDbEntities();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<UserDTO, User>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<User>(user);

                db.Users.Add(data2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new zerohungerDbEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>(); 
            });
            var mapper = new Mapper(config);
            var data2 = mapper.Map<UserDTO>(user);


            return View(data2);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Username,Password,Role,Name,ContactInfo,Location")] UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var db = new zerohungerDbEntities();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<UserDTO, User>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<User>(user); 

                db.Entry(data2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
