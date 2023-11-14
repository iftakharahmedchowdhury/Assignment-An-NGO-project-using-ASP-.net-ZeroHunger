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
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
          
            var db = new zerohungerDbEntities();

            /* var allRequests = db.CollectRequests.ToList();*/
            var allRequests = db.CollectRequests.OrderByDescending(c => c.RequestID).ToList();

            // AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CollectRequest, CollectRequestDTO>()
                    .ForMember(dest => dest.CollectRequestsFooditems, opt => opt.MapFrom(src => src.CollectRequestsFooditems));
                cfg.CreateMap<CollectRequestsFooditem, CollectRequestsFooditemDTO>();
            });

            var mapper = new Mapper(config);
            var adminData = mapper.Map<List<CollectRequestDTO>>(allRequests);

            return View(adminData);

        }

        public ActionResult FoodItems(int requestId)
        {
            var db = new zerohungerDbEntities();
            
                // get all food item against the Request id
                var foodItems = db.CollectRequestsFooditems
                    .Where(item => item.RequestID == requestId)
                    .ToList();

                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CollectRequestsFooditem, CollectRequestsFooditemDTO>();
                });
                var mapper = new Mapper(config);
                var foodItemsDTO = mapper.Map<List<CollectRequestsFooditemDTO>>(foodItems);

                // collect all employee
                var employees = db.Users
                .Where(user => user.Role.Contains("Employee"))
                .ToList();

                ViewBag.Employees = employees;
            // Send employees to the view bag

            var requestStatusAndEmployeeName = db.CollectRequests
          .Where(request => request.RequestID == requestId)
          .Select(request => new
          {
              Status = request.Status,
              AssignedEmployeeName = db.Users
                  .Where(user => user.UserID == request.AssignedEmployeeID)
                  .Select(user => user.Username)
                  .FirstOrDefault()
          })
          .FirstOrDefault();

            // check if employee already assigned
            if (requestStatusAndEmployeeName != null && requestStatusAndEmployeeName.Status == "AssignedEmployee" || requestStatusAndEmployeeName.Status == "completed" || requestStatusAndEmployeeName.Status == "collected")
            {
                ViewBag.Status = "AssignedEmployee";
                ViewBag.AssignedEmployeeName = requestStatusAndEmployeeName.AssignedEmployeeName;
            }

            return View(foodItemsDTO);
            
        }


        [HttpPost]
        public ActionResult AssignEmployee(int requestId, int employeeId)
        {
            var db = new zerohungerDbEntities();
   
                var collectRequest = db.CollectRequests.FirstOrDefault(request => request.RequestID == requestId);

                if (collectRequest != null)
                {
                    collectRequest.AssignedEmployeeID = employeeId;
                    collectRequest.Status = "AssignedEmployee";
                    db.SaveChanges();
                }
                return RedirectToAction("FoodItems", new { requestId = requestId });
            
        }

        public ActionResult RequestForFoodDelivery()
        {
            var db = new zerohungerDbEntities();

            var storedFoodItems = db.StoredFoodItems.ToList();
            return View(storedFoodItems);
        }

        public ActionResult ReadyForDelivery(int requestId,int foodItemId, string itemName, int quantity, DateTime expiryDate, string description)
        {
            // add items in DeliveryInfo seassion
            if (Session["DeliveryInfo"] == null)
            {
                Session["DeliveryInfo"] = new List<DeliveryItem>();
            }

            List<DeliveryItem> deliveryInfo = (List<DeliveryItem>)Session["DeliveryInfo"];

            var existingItem = deliveryInfo.FirstOrDefault(item => item.FoodItemId == foodItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                deliveryInfo.Add(new DeliveryItem
                {
                    RequestId = requestId,
                    FoodItemId = foodItemId,
                    ItemName = itemName,
                    Quantity = quantity,
                    ExpiryDate = expiryDate,
                    Description = description
                });
            }

            Session["DeliveryInfo"] = deliveryInfo;

            return RedirectToAction("RequestForFoodDelivery");
        }

        [HttpGet]
        public ActionResult ViewReadyForDelivery()
        {
            // get all the item stored in DeliveryInfo seassion
            List<DeliveryItem> readyDeliveryItems = Session["DeliveryInfo"] as List<DeliveryItem>;

            if (readyDeliveryItems == null || readyDeliveryItems.Count == 0)
            {
                ViewBag.Message = "No items are ready for delivery.";
            }
            else
            {
                var db = new zerohungerDbEntities();
                    var employees= db.Users
                        .Where(u => u.Role == "Employee")
                        .ToList();
                


                ViewBag.ProductReadyForDeliveryVB = readyDeliveryItems;

                // Creating a list of employee
                ViewBag.EmployeeList = new SelectList(employees, "UserId", "UserName");
            }

            return View("ViewReadyForDelivery");
        }

        [HttpPost]
        public ActionResult SubmitDeliveryConfirmation(DeliveryConfirmation model)
        {
            if (Session["DeliveryInfo"] != null)
            {
                try
                {
                    var deliveryInfo = (List<DeliveryItem>)Session["DeliveryInfo"];

                    using (var db = new zerohungerDbEntities()) 
                    {
                        foreach (var item in deliveryInfo)
                        {
                            var deliveryConfirmation = new DeliveryConfirmation
                            {
                                ConfirmingEmployeeID = model.ConfirmingEmployeeID,
                                RequestID = item.RequestId,
                                DetailsComments = model.DetailsComments,
                                DeliveryLocation = model.DeliveryLocation,
                                DeliveryStatus = "Processing",
                                ItemName = item.ItemName,
                                Quantity = item.Quantity,
                                ExpiryDate = item.ExpiryDate,
                            };

                            // Add the new item to the DbSet
                            db.DeliveryConfirmations.Add(deliveryConfirmation);

                            // after add remove item from storedFoodItem
                            var storedFoodItem = db.StoredFoodItems.FirstOrDefault(sfi => sfi.RequestID == item.RequestId && sfi.FoodItemID == item.FoodItemId);
                            if (storedFoodItem != null)
                            {
                                db.StoredFoodItems.Remove(storedFoodItem);
                            }
                        }

                        db.SaveChanges();
                    }

                    Session["DeliveryInfo"] = null;

                    return RedirectToAction("DeliveryConfirmationSuccess");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }



        public ActionResult DeliveryConfirmationSuccess()
        {
            return View();
        }


        public ActionResult Logout()
        {

            if (Request.Cookies["EmployeeInfo"] != null)
            {
                var UserCookie = new HttpCookie("EmployeeInfo");
                UserCookie.Expires = DateTime.Now.AddYears(-1); // remove user info from cookie so that user can logout
                Response.Cookies.Add(UserCookie);
            }


            return RedirectToAction("Login", "Users");
        }



    }
}