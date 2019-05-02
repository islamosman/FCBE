using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel
{
    public partial class Vehicles
    {
        public List<LukUpModel> vCategory { get; set; }

        public List<LukUpModel> vBrand { get; set; }

        public List<LukUpModel> AreasList { get; set; }


        public int AreaId { get; set; }

        public bool InService { get; set; }
        public bool InRide { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public decimal BatteryStatus { get; set; }
    }
}
