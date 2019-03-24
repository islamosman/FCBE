using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.Helper;
using Fly.Resources;

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
                        responseObj = AddUpdate(tripModel);
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
    }
}
