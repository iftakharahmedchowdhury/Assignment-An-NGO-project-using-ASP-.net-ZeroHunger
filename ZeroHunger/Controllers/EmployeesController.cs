using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.EF;

namespace ZeroHunger.Controllers
{
    public class EmployeesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CollectOrder()
        {
            int employeeUserId = Convert.ToInt32(Request.Cookies["EmployeeInfo"]["UserId"]);

            var db = new zerohungerDbEntities();

            /* var collectOrders = db.CollectRequests
                 .Where(order => order.AssignedEmployeeID == employeeUserId)
                 .ToList();*/
            var collectOrders = db.CollectRequests
                .Where(order => order.AssignedEmployeeID == employeeUserId).OrderByDescending(order=>order.RequestID)
                .ToList();
            
            // Store individual resturent id
            var restaurantUserIds = collectOrders.Select(order => order.ResturentUserID).Distinct().ToList();

            // Store resturename in ViewBag
            ViewBag.RestaurantNames = db.Users
                .Where(user => restaurantUserIds.Contains(user.UserID))
                .ToDictionary(user => user.UserID, user => user.Name);

            return View("CollectOrderFromResturent", collectOrders);

        }

        public ActionResult CollectOrderFromResturent(List<CollectRequest> collectOrders)
        {
            return View(collectOrders);
        }


        public ActionResult CollectionTime(int requestId)
        {
            var db = new zerohungerDbEntities();

            // 
            var collectRequest = db.CollectRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (collectRequest != null)
            {
                // insert current time and date
                collectRequest.CollectionTime = DateTime.Now;

                collectRequest.Status = "collected";

                db.SaveChanges();

                return RedirectToAction("CollectOrder");
            }

            return HttpNotFound();
        }

        public ActionResult CompletionTime(int requestId)
        {
            var db = new zerohungerDbEntities();

            var collectRequest = db.CollectRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (collectRequest != null)
            {
                collectRequest.CompletionTime = DateTime.Now;

                collectRequest.Status = "completed";

                // get all the food item from CollectRequestsFooditems
                var foodItems = db.CollectRequestsFooditems
                    .Where(item => item.RequestID == requestId)
                    .ToList();

                // store all the items in StoredFoodItems
                foreach (var foodItem in foodItems)
                {
                    var storedFoodItem = new StoredFoodItem
                    {
                        RequestID = foodItem.RequestID,
                        ItemName = foodItem.ItemName,
                        Quantity = foodItem.Quantity,
                        ExpiryDate = foodItem.ExpiryDate,
                        Description = foodItem.Description
                    };

                    db.StoredFoodItems.Add(storedFoodItem);
                }

                db.SaveChanges();

                return RedirectToAction("CollectOrder");
            }

            return HttpNotFound();
        }

        public ActionResult DeliveryOrder()
        {
            int employeeUserId = Convert.ToInt32(Request.Cookies["EmployeeInfo"]["UserId"]);

            var db = new zerohungerDbEntities();

            var collectOrders = db.DeliveryConfirmations
                .Where(order => order.ConfirmingEmployeeID == employeeUserId).OrderByDescending(order => order.ConfirmationID)
                .ToList();


            return View(collectOrders);
        }

        public ActionResult UpdateConfirmationTime(int confirmationId)
        {
            var db = new zerohungerDbEntities();
            var confirmation = db.DeliveryConfirmations.Find(confirmationId);
            if (confirmation != null)
            {
                
                confirmation.ConfirmationTime = DateTime.Now;

                confirmation.DeliveryStatus = "Collected";

                db.SaveChanges();
            }

            return RedirectToAction("DeliveryOrder"); 
        }

       
        public ActionResult UpdateCompletionTime(int confirmationId)
        {
            var db = new zerohungerDbEntities();
            var confirmation = db.DeliveryConfirmations.Find(confirmationId);
            if (confirmation != null)
            {

                confirmation.CompletionTime = DateTime.Now;

                confirmation.DeliveryStatus = "Completed";

                db.SaveChanges();
            }

            return RedirectToAction("DeliveryOrder"); 
        }

        public ActionResult Logout()
        {

            if (Request.Cookies["EmployeeInfo"] != null)
            {
                var UserCookie = new HttpCookie("EmployeeInfo");
                UserCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(UserCookie);
            }

            return RedirectToAction("Login", "Users");
        }
    }
}