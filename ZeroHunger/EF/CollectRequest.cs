//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeroHunger.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class CollectRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CollectRequest()
        {
            this.CollectRequestsFooditems = new HashSet<CollectRequestsFooditem>();
            this.DeliveryConfirmations = new HashSet<DeliveryConfirmation>();
            this.StoredFoodItems = new HashSet<StoredFoodItem>();
        }
    
        public int RequestID { get; set; }
        public int ResturentUserID { get; set; }
        public System.DateTime MaxPreserveTime { get; set; }
        public string Status { get; set; }
        public string CollectionAddress { get; set; }
        public Nullable<int> AssignedEmployeeID { get; set; }
        public Nullable<System.DateTime> CollectionTime { get; set; }
        public Nullable<System.DateTime> CompletionTime { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CollectRequestsFooditem> CollectRequestsFooditems { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryConfirmation> DeliveryConfirmations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoredFoodItem> StoredFoodItems { get; set; }
    }
}
