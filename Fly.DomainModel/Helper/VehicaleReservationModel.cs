using Fly.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel.Helper
{
    public class VehicaleReservationModel
    {
        public int? tripId { get; set; }
        public int riderId { get; set; }

        public string vehicleId { get; set; }
        public string qrStr { get; set; }
        public TripType reservationEnum { get; set; }
    }
}
