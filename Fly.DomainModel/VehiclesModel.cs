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
    
    [MetadataType(typeof(Metadata.VehiclesModelMetadata))]
     [DataContract(IsReference = true)]
     [KnownType(typeof(VehiclesModel))]
     public partial class VehiclesModel
    {
        public VehiclesModel()
        {
            this.VehicleSpecs = new HashSet<VehicleSpecs>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
    
        public virtual VehiclesBrand VehiclesBrand { get; set; }
        public virtual ICollection<VehicleSpecs> VehicleSpecs { get; set; }
    }
}