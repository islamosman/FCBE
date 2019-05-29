using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.Helper;
using Fly.Resources;
using System.Data.Entity;

namespace Fly.BLL
{
    public class TripRepository : MainRepository<Trips>
    {
        public override RequestResponse AddUpdate(Trips entity)
        {
            Validate(entity);
            if (responseObj.ErrorMessages.Count > 0)
            {
                return responseObj;
            }
            try
            {
                if (entity.Id > 0)
                {
                    Attach(entity);
                }
                else
                {
                    Add(entity);
                }

                Save();

                responseObj.ReturnedObject = entity;
                responseObj.ResponseIdStr = entity.Id.ToString();
            }
            catch (Exception ex)
            {
                responseObj.ErrorMessages.Add("fillAlldata", ex.InnerException.Message);
            }

            return responseObj;
        }

        public override Trips GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public override void Validate(Trips entity)
        {
            // Required
            if (entity.VehicleId <= 0 || entity.RiderId <= 0)
            {
                responseObj.ErrorMessages.Add("fillAlldata", "Fill all data");
            }
        }

        public int? GetLastVehicalOpenTrip(string vId)
        {
            using (VehicleStatusRepository vsRepo = new VehicleStatusRepository())
            {
                VehicleStatus vsModel = vsRepo.GetByQR(vId);
                if (vsModel != null)
                {
                    return _objectSet.FirstOrDefault(x => x.VehicleId == vsModel.VehicleId && x.IsDone == false)?.Id;
                }
            }
            return null;
        }

        public RequestResponse GetBalance(int userId)
        {
            List<Trips> userTrips = _objectSet.Where(x => x.RiderId == userId).ToList();

            decimal? totalRideAmount = userTrips.Where(x => x.IsDone == true).Sum(x => x.Amount);
            decimal? totalRidePaied = userTrips.Where(x => x.IsDone == true && x.IsPaid == true).Sum(x => x.Amount);

            responseObj.ReturnedObject = new
            {
                Total = totalRideAmount,
                Paied = totalRidePaied
            };
            return responseObj;
        }

        public RequestResponse TripRegister(VehicaleReservationModel data)
        {


            Trips tripModel = new Trips()
            {
                VehicleId = int.Parse(data.vehicleId),
                RiderId = data.riderId
            };

            switch (data.reservationEnum)
            {
                case TripType.Start:

                    // check if v QR found
                    using (VehicleRepository vRepo = new VehicleRepository())
                    {
                        if (!vRepo.IsAvilableByBarcode(data.qrStr, int.Parse(data.vehicleId)))
                        {
                            responseObj.ErrorMessages.Add("nodata", "QR is wrong");
                            return responseObj;
                        }
                    }

                    //if (!data.tripId.HasValue)
                    //{
                    //    responseObj.ErrorMessages.Add("tripId", OperationLP.InvalidData);
                    //}
                    //else
                    //{
                    //tripModel = GetById(data.tripId.Value);
                    tripModel.CreatedDate = DateTime.Now;
                    tripModel.StartTime = DateTime.Now;
                    tripModel.TempRequest = false;
                    responseObj = AddUpdate(tripModel);

                    using (VehicleStatusRepository vsRepo = new VehicleStatusRepository())
                    {
                        vsRepo.UpdateInRide(int.Parse(data.vehicleId));
                    }
                    //}
                    break;
                case TripType.End:
                    if (!data.tripId.HasValue)
                    {
                        responseObj.ErrorMessages.Add("tripId", OperationLP.InvalidData);
                    }
                    else
                    {
                        tripModel = GetById(data.tripId.Value);
                        tripModel.EndTime = DateTime.Now;
                        tripModel.IsDone = true;
                        TimeSpan span = DateTime.Parse(tripModel.EndTime.ToString()) - DateTime.Parse(tripModel.StartTime.ToString());
                        double totalMinutes = span.TotalMinutes;
                        double totalHours = span.TotalHours;
                        double totalSecounds = span.TotalSeconds;

                        //tripModel.Duration = string.Format("{0}:{1}:{2}", totalHours, totalMinutes, totalSecounds);
                        tripModel.Duration = Math.Round(totalMinutes, 2).ToString();

                        double unloackAmount = double.Parse(System.Configuration.ConfigurationManager.AppSettings["unlockAmount"].ToString());
                        double minuteAmount = double.Parse(System.Configuration.ConfigurationManager.AppSettings["minuteAmount"].ToString());


                        decimal totalAmount = Math.Round(decimal.Parse((unloackAmount + (totalMinutes * minuteAmount)).ToString()), 2);
                        tripModel.Amount = totalAmount;

                        if (data.PromoCodeId > 0)
                        {
                            decimal promoDiscount = 0;
                            using (PromoCodeRepository promoRepo = new PromoCodeRepository())
                            {
                                PromoCode promoModel = promoRepo.GetById(data.PromoCodeId);
                                if (promoModel != null)
                                {
                                    promoDiscount = promoModel.Percentage;
                                }
                            }
                            tripModel.NetAmount = totalAmount - ((totalAmount * promoDiscount) / 100);
                        }
                        else
                        {
                            tripModel.NetAmount = totalAmount;
                        }


                        responseObj = AddUpdate(tripModel);

                        using (VehicleStatusRepository vsRepo = new VehicleStatusRepository())
                        {
                            vsRepo.UpdateInRide(int.Parse(data.vehicleId), false, data.needPrepare);
                        }
                    }
                    break;
                case TripType.TempRequest:
                    tripModel.CreatedDate = DateTime.Now;
                    tripModel.TempRequest = true;
                    responseObj = AddUpdate(tripModel);
                    break;
                case TripType.Cancel:
                    if (!data.tripId.HasValue)
                    {
                        responseObj.ErrorMessages.Add("tripId", OperationLP.InvalidData);
                    }
                    else
                    {
                        tripModel = GetById(data.tripId.Value);
                        tripModel.IsDone = false;
                        tripModel.IsCancel = true;
                        responseObj = AddUpdate(tripModel);
                    }
                    break;
                default:
                    break;
            }
            return responseObj;
        }

        public RequestResponse GetTripStatus(int tripId)
        {
            Trips currentTrip = _objectSet.FirstOrDefault(x => x.Id == tripId);
            if (currentTrip != null)
            {
                if (currentTrip.Id > 0)
                {
                    TimeSpan span = DateTime.Parse(currentTrip.EndTime.HasValue ? currentTrip.EndTime.ToString() : DateTime.Now.ToString())
                           - DateTime.Parse(currentTrip.StartTime.ToString());
                    double totalMinutes = span.TotalMinutes;
                    double totalHours = span.TotalHours;
                    double totalSecounds = span.TotalSeconds;

                    if (currentTrip.IsDone)
                    {
                        responseObj.ReturnedObject = new
                        {
                            TripId = currentTrip.Id,
                            vId = currentTrip.VehicleId,
                            IsDone = currentTrip.IsDone,
                            rate = currentTrip.Rate,
                            IsCancel = currentTrip.IsCancel,
                            StartDate = currentTrip.StartTime.ToString("dd/MM/yyyy , HH:mm:ss tt"),
                            EndDate = currentTrip.EndTime.HasValue ? currentTrip.EndTime.Value.ToString("dd/MM/yyyy , HH:mm:ss tt") : "",
                            Duration = currentTrip.Duration.ToString(),
                            totalSecounds = totalSecounds,
                            Amount = Math.Round(currentTrip.NetAmount.Value, 2)
                        };
                    }
                    else
                    {
                        double unloackAmount = double.Parse(System.Configuration.ConfigurationManager.AppSettings["unlockAmount"].ToString());
                        double minuteAmount = double.Parse(System.Configuration.ConfigurationManager.AppSettings["minuteAmount"].ToString());
                       
                        responseObj.ReturnedObject = new
                        {
                            TripId = currentTrip.Id,
                            vId = currentTrip.VehicleId,
                            IsDone = currentTrip.IsDone,
                            rate = currentTrip.Rate,
                            IsCancel = currentTrip.IsCancel,
                            StartDate = currentTrip.StartTime.ToString("dd/MM/yyyy , HH:mm:ss tt"),
                            EndDate = currentTrip.EndTime.HasValue ? currentTrip.EndTime.Value.ToString("dd/MM/yyyy , HH:mm:ss tt") : "",
                            Duration = Math.Round(totalMinutes, 2).ToString(),
                            totalSecounds = totalSecounds,
                            Amount = Math.Round(decimal.Parse((unloackAmount + (totalMinutes * minuteAmount)).ToString()), 2)
                        };

                    }
                }
                else
                {
                    responseObj.ErrorMessages.Add("nod", Resources.OperationLP.TblNoData);
                }
            }
            else
            {
                responseObj.ErrorMessages.Add("nod", Resources.OperationLP.TblNoData);
            }

            return responseObj;
        }

        public RequestResponse TripPaymentId(int tripId, int orderId)
        {
            Trips currentTrip = _objectSet.FirstOrDefault(x => x.Id == tripId);
            if (currentTrip != null)
            {
                currentTrip.WeAcceptOrderId = orderId;
                currentTrip.IsPaid = true;
                AddUpdate(currentTrip);
            }
            else
            {
                responseObj.ErrorMessages.Add("notrip", "Trip Not Found");
            }

            return responseObj;
        }

        public RequestResponse TripPaymentDone(int orderId)
        {
            Trips currentTrip = _objectSet.FirstOrDefault(x => x.WeAcceptOrderId == orderId);
            if (currentTrip != null)
            {
                currentTrip.IsPaid = true;
                AddUpdate(currentTrip);
            }
            else
            {
                responseObj.ErrorMessages.Add("notrip", "Trip Not Found");
            }

            return responseObj;
        }

        public RequestResponse UpdateTripRate(int tripId, int rate, bool isRepair)
        {
            Trips currentTrip = GetById(tripId);
            currentTrip.Rate = byte.Parse(rate.ToString());
            AddUpdate(currentTrip);

            if (isRepair)
            {
                using (VehicleStatusRepository vStatus = new VehicleStatusRepository())
                {
                    VehicleStatus currentVStatusModel = vStatus.GetById(currentTrip.VehicleId);
                    currentVStatusModel.InService = isRepair;

                    vStatus.AddUpdate(currentVStatusModel);
                }
            }
            return responseObj;
        }

        public RequestResponse GetAllByUser(int userId)
        {
            responseObj.ReturnedObject = _objectSet.Include(x => x.Vehicles).Where(x => x.RiderId == userId).OrderByDescending(x => x.EndTime).ToList();
            return responseObj;
        }

        public RequestResponse GetHistoryByUser(int userId)
        {
            List<Trips> tripsList = _objectSet.Include(x => x.Vehicles).Where(x => x.RiderId == userId).OrderByDescending(x => x.EndTime).ToList();

            if (tripsList.Count > 0)
            {
                responseObj.ReturnedObject = tripsList.ToList().Select(x => new
                {
                    x.Amount,
                    x.Duration,
                    StartTime = x.StartTime.ToString("dd/MM/yyyy hh:mm tt"),
                    EndTime = x.EndTime.HasValue ? x.EndTime.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                    x.Vehicles.Name,
                    IsPaid = x.IsPaid == true ? true : false,
                    IsDone = x.IsDone == true ? true : false,
                    x.NetAmount
                });
            }
            return responseObj;
        }
    }
}
