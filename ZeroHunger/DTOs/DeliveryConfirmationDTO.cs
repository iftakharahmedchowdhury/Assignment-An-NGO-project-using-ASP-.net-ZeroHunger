using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeroHunger.DTOs
{
    public class DeliveryConfirmationDTO
    {
        public int ConfirmationID { get; set; }
        public int ConfirmingEmployeeID { get; set; }
        public int RequestID { get; set; }
        public Nullable<System.DateTime> ConfirmationTime { get; set; }
        public string DetailsComments { get; set; }
        public string DeliveryLocation { get; set; }
        public string DeliveryStatus { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public Nullable<System.DateTime> CompletionTime { get; set; }
    }
}