using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeroHunger.DTOs
{
    public class CollectRequestDTO
    {
        public int RequestID { get; set; }
        public int ResturentUserID { get; set; }
        public System.DateTime MaxPreserveTime { get; set; }
        public string Status { get; set; }
        public string CollectionAddress { get; set; }
        public Nullable<int> AssignedEmployeeID { get; set; }
        public Nullable<System.DateTime> CollectionTime { get; set; }
        public Nullable<System.DateTime> CompletionTime { get; set; }

        // Initialize the list in the constructor
        public CollectRequestDTO()
        {
            CollectRequestsFooditems = new List<CollectRequestsFooditemDTO>();
        }

        public List<CollectRequestsFooditemDTO> CollectRequestsFooditems { get; set; }
    

}
}