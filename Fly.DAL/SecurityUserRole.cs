//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fly.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class SecurityUserRole
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        public virtual SecurityRole SecurityRole { get; set; }
        public virtual SecurityUser SecurityUser { get; set; }
    }
}
