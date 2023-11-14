using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeroHunger.DTOs
{
    public class StoredFoodItemDTO
    {
        public int FoodItemID { get; set; }
        public int RequestID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public string Description { get; set; }
    }
}