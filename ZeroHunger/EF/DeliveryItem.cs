using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeroHunger.EF
{
    public class DeliveryItem
    {
        public int RequestId { get; set; }
        public int FoodItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Description { get; set; }
    }
}