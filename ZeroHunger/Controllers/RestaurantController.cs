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
   
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp([Bind(Include = "UserID,Username,Password,Role,Name,ContactInfo,Location")] UserDTO c)
        {
            var db = new zerohungerDbEntities();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = new Mapper(config);
            var conData = mapper.Map<User>(c);
            db.Users.Add(conData);
            db.SaveChanges();
            return RedirectToAction("Login","Users");

        }


        public ActionResult Deshboard()
        {

            int userId = 0;
            var db = new zerohungerDbEntities();

            if (Request.Cookies["UserInfo"] != null && int.TryParse(Request.Cookies["UserInfo"]["UserId"], out userId))
            {
                /*var userOrders = db.CollectRequests.Where(o => o.ResturentUserID == userId).ToList();
                return View(userOrders);*/
                var userOrders = db.CollectRequests
               .Where(o => o.ResturentUserID == userId).OrderByDescending(o => o.RequestID).ToList();

                return View(userOrders);

            }


            return RedirectToAction("Login","Users");


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

        //submit the collection requset
        [HttpPost]
        public ActionResult CollectRequest(CollectRequestDTO collectRequest)
        {
            // adding collect request in CollectRequest seassion
            Session["CollectRequest"] = collectRequest;

            // go to add item page
            return RedirectToAction("AddItem");
        }

       
        public ActionResult AddItem()
        {
            return View();
        }

        // submit Add item
        [HttpPost]
        public ActionResult AddItem(CollectRequestsFooditemDTO item)
        {
            // Retrieve collect request from session
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO;

            // make sure that i have collectRequest seassion
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            // check CollectRequestsFooditems is null or nor if null initialize
            if (collectRequest.CollectRequestsFooditems == null)
            {
                collectRequest.CollectRequestsFooditems = new List<CollectRequestsFooditemDTO>();
            }

            // add item in CollectRequestsFooditems
            collectRequest.CollectRequestsFooditems.Add(item);

            // update seassion so that we also can store collect request items
            Session["CollectRequest"] = collectRequest;

            
            return RedirectToAction("AddItem");
        }

       
        public ActionResult ShowResItemDetails()
        {
            // Retrieve data of CollectRequest seassion 
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO; //casting retrive value to CollectRequestDTO type.

            
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            return View(collectRequest);
        }

        public ActionResult ConfirmRequest()
        {
            // Get all data from seassion CollectRequest
            var collectRequest = Session["CollectRequest"] as CollectRequestDTO;

            // Ensure collect request exists in session
            if (collectRequest == null)
            {
                return RedirectToAction("CollectRequest");
            }

            // Get user id from cookie name it integer because cookie store string type data
            var userId = Convert.ToInt32(Request.Cookies["UserInfo"]["UserId"]);

            // Save the resource in database
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

                // After save request id in db it ensure it can retrive request id
                var requestId = newRequest.RequestID;

                // Save item against the request id in CollectRequestsFoodItem
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

            // After complete the store data in database clear seassion
            Session["CollectRequest"] = null;

            return RedirectToAction("Deshboard", "Restaurant");
        }

    }
}