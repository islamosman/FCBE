using System.ComponentModel.DataAnnotations;

namespace Fly.DomainModel.Metadata
{
    internal class VehiclesMetadata
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PlateNo { get; set; }
        [Required(ErrorMessage = "Required")]
        public int AreaId { get; set; }
    }
}