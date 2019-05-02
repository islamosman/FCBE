using System.ComponentModel.DataAnnotations;

namespace Fly.DomainModel.Metadata
{
    internal class PromoCodeMetadata
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal Percentage { get; set; }
    }
}