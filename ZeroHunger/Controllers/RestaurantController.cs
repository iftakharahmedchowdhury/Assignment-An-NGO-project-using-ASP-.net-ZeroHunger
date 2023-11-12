using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DTOs;
using ZeroHunger.EF;

namespace ZeroHunger.Controllers
{
    public class RestaurantController : Controller
    {
        // GET: Restaurant
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Index(UserDTO User)
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

                return RedirectToAction("Index", "Admin");

            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
        }


        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserDTO c)
        {
            var db = new zerohungerDbEntities();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = new Mapper(config);
            var conData = mapper.Map<User>(c);
            db.Users.Add(conData);
            db.SaveChanges();
            return RedirectToAction("Index");

        }


        public ActionResult Deshboard()
        {

            int userId = 0;
            var db = new zerohungerDbEntities();

            if (Request.Cookies["UserInfo"] != null && int.TryParse(Request.Cookies["UserInfo"]["UserId"], out userId))
            {
                /*var userOrders = db.OrderDetails.Where(o => o.CusId == userId).ToList();
                return View(userOrders);*/
                return View();
            }


            return View("Error");


        }

        public ActionResult Logout()
        {

            if (Request.Cookies["UserInfo"] != null)
            {
                var UserCookie = new HttpCookie("UserInfo");
                UserCookie.Expires = DateTime.Now.AddYears(-1); // remove user info from cookie so that user can logout
                Response.Cookies.Add(UserCookie);
            }


            return RedirectToAction("Login","Users");
        }














        public ActionResult CollectRequest()
        {
            CollectRequestDTO collectRequest = new CollectRequestDTO
            {
                Status = "Processing" // Set default status to "Processing"
            };

            return View(collectRequest);
        }

        // Action to handle the submission of the collect request form
        [HttpPost]
        public ActionResult CollectRequest(CollectRequestDTO collectRequest)
        {
            // Save collect request details to session
            Session["CollectRequest"] = collectRequest;

            // Redirect to the page for adding items
            return RedirectToAction("AddItem");
        }

        // Action to display the form for adding items to the collect request
        public ActionResult AddItem()
        {
            return View();
        }

        // Action to handle the submission of the item form
        [HttpPost]
        public ActionResult AddItem(CollectRequestsFooditemDTO item)
        {
            // Retrieve collect request from session
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO;

            // Ensure collect request exists in session
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            // Initialize the list if it doesn't exist
            if (collectRequest.CollectRequestsFooditems == null)
            {
                collectRequest.CollectRequestsFooditems = new List<CollectRequestsFooditemDTO>();
            }

            // Add the item to the collect request
            collectRequest.CollectRequestsFooditems.Add(item);

            // Save updated collect request to session
            Session["CollectRequest"] = collectRequest;

            // Redirect to the page for adding more items or displaying details
            return RedirectToAction("AddItem");
        }

        // Action to show details of the collected items
        public ActionResult ShowResItemDetails()
        {
            // Retrieve collect request from session
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO;

            // Ensure collect request exists in session
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            return View(collectRequest);
        }





        public ActionResult ConfirmRequest()
        {
            // Retrieve collect request from session
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO;

            // Ensure collect request exists in session
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            // Get user information from the cookie
            var userId = Convert.ToInt32(Request.Cookies["UserInfo"]["UserId"]);

            // Save the main request details in the CollectRequest table
            using (var db = new zerohungerDbEntities())
            {
                var newRequest = new CollectRequest
                {
                    ResturentUserID = userId,
                    MaxPreserveTime = collectRequest.MaxPreserveTime,
                    Status = collectRequest.Status,
                    CollectionAddress = collectRequest.CollectionAddress
                };

                db.CollectRequests.Add(newRequest);
                db.SaveChanges();

                // Retrieve the generated RequestID
                var requestId = newRequest.RequestID;

                // Save the associated items in the CollectRequestsFoodItem table
                foreach (var item in collectRequest.CollectRequestsFooditems)
                {
                    var newItem = new CollectRequestsFooditem
                    {
                        RequestID = requestId,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        ExpiryDate = item.ExpiryDate,
                        Description = item.Description
                    };

                    db.CollectRequestsFooditems.Add(newItem);
                }

                db.SaveChanges();
            }

            // Clear the session after storing data in the database
            Session["CollectRequest"] = null;

            return RedirectToAction("Index", "Home"); // Redirect to the home page or another suitable page
        }







    }
}