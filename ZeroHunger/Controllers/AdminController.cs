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
        // GET: Admin
        public ActionResult Index()
        {
          
            var db = new zerohungerDbEntities();

            // Retrieve all requests
            var allRequests = db.CollectRequests.ToList();

            // AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CollectRequest, CollectRequestDTO>()
                    .ForMember(dest => dest.CollectRequestsFooditems, opt => opt.MapFrom(src => src.CollectRequestsFooditems));
                cfg.CreateMap<CollectRequestsFooditem, CollectRequestsFooditemDTO>();
            });

            // Create a mapper instance
            var mapper = new Mapper(config);

            // Map CollectRequest entities to CollectRequestDTO
            var adminData = mapper.Map<List<CollectRequestDTO>>(allRequests);

            return View(adminData);

        }

        public ActionResult FoodItems(int requestId)
        {
            var db = new zerohungerDbEntities();
            
                // Retrieve food items based on the requestId
                var foodItems = db.CollectRequestsFooditems
                    .Where(item => item.RequestID == requestId)
                    .ToList();

                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CollectRequestsFooditem, CollectRequestsFooditemDTO>();
                });
                var mapper = new Mapper(config);
                var foodItemsDTO = mapper.Map<List<CollectRequestsFooditemDTO>>(foodItems);

                // Retrieve employees with the role "Employee"
                var employees = db.Users
                .Where(user => user.Role.Contains("Employee"))
                .ToList();

                ViewBag.Employees = employees;
            // Send employees to the view






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

            // Check if the status is "AssignedEmployee"
            if (requestStatusAndEmployeeName != null && requestStatusAndEmployeeName.Status == "AssignedEmployee")
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
            
                // Retrieve the CollectRequest entity
                var collectRequest = db.CollectRequests.FirstOrDefault(request => request.RequestID == requestId);

                if (collectRequest != null)
                {
                    // Update the AssignedEmployeeID
                    collectRequest.AssignedEmployeeID = employeeId;
                    collectRequest.Status = "AssignedEmployee";

                    // Save changes to the database
                    db.SaveChanges();
                }

                // Redirect back to the FoodItems action
                return RedirectToAction("FoodItems", new { requestId = requestId });
            
        }





    }
}