//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fly.DomainModel
{
    using System;
    using System.Collections.Generic;
     using System.Runtime.Serialization;
     using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(Metadata.SecurityRoleMetadata))]
     [DataContract(IsReference = true)]
     [KnownType(typeof(SecurityRole))]
     public partial class SecurityRole
    {
        public SecurityRole()
        {
            this.SecurityUserRole = new HashSet<SecurityUserRole>();
        }
    
        public int Id { get; set; }
        public string RoleNameA { get; set; }
        public string RoleNameE { get; set; }
        public bool FullDataAccess { get; set; }
    
        public virtual ICollection<SecurityUserRole> SecurityUserRole { get; set; }
    }
}