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
        /*private zerohungerDbEntities db = new zerohungerDbEntities(); // we also can initialize this way so that we dont have to initialize in every section */

        // GET: Users
       

        [HttpGet]
        public ActionResult Index()
        {
            var db = new zerohungerDbEntities();
            int userId = 0;
            if (Request.Cookies["AdminInfo"] != null && int.TryParse(Request.Cookies["AdminInfo"]["UserId"], out userId))
            {
                var data = db.Users.ToList();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<User, UserDTO>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<List<UserDTO>>(data);
                return View(data2);

            }
            return RedirectToAction("Login", "Users");
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
            //if role is Restaurant it will go to Restaurant deshboard 
            if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Restaurant")
            {
                //Creating UserInfo cookie so that user information can store in cookie
                HttpCookie UserCookie = new HttpCookie("UserInfo");
                UserCookie["Username"] = existingUser.Username;
                UserCookie["UserId"] = existingUser.UserID.ToString();//cookie can store only string
                UserCookie.Expires = DateTime.Now.AddMinutes(5); // set cookie time. after that it will remove
                Response.Cookies.Add(UserCookie);

                return RedirectToAction("Deshboard", "Restaurant");
            }
            //if role is Employee it will go to Employee deshboard 
            else if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Employee")
            {
                HttpCookie EmployeeCookie = new HttpCookie("EmployeeInfo");
                EmployeeCookie["Username"] = existingUser.Username;
                EmployeeCookie["UserId"] = existingUser.UserID.ToString();
                EmployeeCookie.Expires = DateTime.Now.AddMinutes(5); 
                Response.Cookies.Add(EmployeeCookie);

                return RedirectToAction("Index", "Employees");

            }
            //if role is Admin it will go to Admin deshboard 
            else if (existingUser != null && conData.Password == existingUser.Password && existingUser.Role == "Admin")
            {
                HttpCookie AdminCookie = new HttpCookie("AdminInfo");
                AdminCookie["Username"] = existingUser.Username;
                AdminCookie["UserId"] = existingUser.UserID.ToString();
                AdminCookie.Expires = DateTime.Now.AddMinutes(5); 
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
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var db = new zerohungerDbEntities();
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var db = new zerohungerDbEntities();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
