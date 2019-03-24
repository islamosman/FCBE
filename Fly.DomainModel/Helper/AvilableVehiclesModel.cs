using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel.Helper
{
    public class AvilableVehiclesModel
    {
        public int id { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string Name { get; set; }
        public bool avilable { get; set; }
        public string description { get; set; }
        public string iconImageEnum { get; set; }
        public string ImageUrl { get; set; }
    }
}
