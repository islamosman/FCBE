using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.BLL;
using Fly.DomainModel;
using System.Net;
using System.Xml.Linq;

namespace Fly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           // var diswtance = distance(30.1703207, 31.226648, 30.1617312, 31.2280126, char.Parse("K"));
           // var distandce = DistanceTo(30.1703207, 31.226648, 30.1617312, 31.2280126);

           //// string address = "Alexandria Agriculture Rd, Madinet Qelyoub, Qalyoub, Al Qalyubia Governorate";
           // //string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBLKRh7JfikPylbNdGfTiDbe6zut1yabxo&address={0}&sensor=false", Uri.EscapeDataString(address));

           // //WebRequest request = WebRequest.Create(requestUri);
           // //WebResponse response = request.GetResponse();
           // //XDocument xdoc = XDocument.Load(response.GetResponseStream());

           // //XElement result = xdoc.Element("GeocodeResponse").Element("result");
           // //XElement locationElement = result.Element("geometry").Element("location");
           // //XElement lat = locationElement.Element("lat");
           // //XElement lng = locationElement.Element("lng");

           // var sCoord = new System.Device.Location.GeoCoordinate(30.1703207, 31.226648);
           // var eCoord = new System.Device.Location.GeoCoordinate(30.1617312, 31.2280126);

           // var sss= sCoord.GetDistanceTo(eCoord);
            using (VehicleRepository scoterRepo = new VehicleRepository())
            {
                scoterRepo.Validate(new Vehicles());
                return View(scoterRepo.GetAll().ToList());
            }
        }
        public double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }
        [HttpGet]
        public ActionResult AddNew(int? scotterId)
        {
            Vehicles scoModel = new Vehicles();
            
            return View(scoModel);
        }

        [HttpPost]
        public ActionResult AddNew(Vehicles model)
        {
            using (VehicleRepository scoterRepo = new VehicleRepository())
            {
                scoterRepo.Add(model);
            }
            return RedirectToAction("AddNew", new { scotterId = model.Id });
        }
    }
}
