//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _5Bites.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Store
    {
        public Store()
        {
            this.EmployeeStores = new HashSet<EmployeeStore>();
            this.Transactions = new HashSet<Transaction>();
        }
    
        public int Id { get; set; }
        public Nullable<decimal> Bank { get; set; }
    
        public virtual ICollection<EmployeeStore> EmployeeStores { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
