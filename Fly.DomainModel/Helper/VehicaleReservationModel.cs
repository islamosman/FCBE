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
        public int PromoCodeId { get; set; }

        public int rateValue { get; set; }
        public bool needPrepare { get; set; }

        public string vehicleId { get; set; }
        public string qrStr { get; set; }
        public TripType reservationEnum { get; set; }
    }


    public class SubscriptionModel
    {
        public int? Id { get; set; }
        public int riderId { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }

        public string Location { get; set; }

        public string Lng { get; set; }
        public string Lat { get; set; }

        public int DaysCount { get; set; }
        public string DateTimeStr { get; set; }

        public string DateStr{ get; set; }
        public string TimeStr { get; set; }

        public string PromoCodeName { get; set; }

        public int? PromoCodeId { get; set; }
        public string PayMobId { get; set; }
    }
}
