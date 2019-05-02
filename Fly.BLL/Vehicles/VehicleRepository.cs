using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel;
using Fly.DomainModel.Helper;
using Fly.Resources;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Fly.BLL
{
    public class VehicleRepository : MainRepository<Vehicles>
    {
        public List<Vehicles> get(int skip, int take, String Searchtoken, string column, string sortColumnDir)
        {
            if (column == "VName")
            {
                column = "Name";
            }


            Expression<Func<Vehicles, bool>> predicate = SearchOnVehicles(Searchtoken);

            return _objectSet.Where(predicate)
                //.Include(x => x.VehicleSpecs)
                .Include(x => x.VehicleStatus).OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take)
                  .ToList();
        }

        public List<Vehicles> getAllForDash()
        {
            return _objectSet.Include(x => x.VehicleStatus).ToList().Select(x => new Vehicles
            {
                Name = x.Name,
                Lat = x.VehicleStatus.LatV,
                Long = x.VehicleStatus.LongV,
                InRide = x.VehicleStatus.InRide,
                InService = x.VehicleStatus.InService,
                PlateNo = x.PlateNo
            }).ToList();
        }

        public Expression<Func<Vehicles, bool>> SearchOnVehicles(String Searchtoken)
        {
            bool flag = false;
            Expression<Func<Vehicles, bool>> predicate;
            predicate = PredicateBuilder.True<Vehicles>();

            //predicate = predicate.And(i => i. == isPhoto);
            //  flag = true;
            if (!string.IsNullOrEmpty(Searchtoken))
            {
                predicate = predicate.And(s => s.Name.Contains(Searchtoken));
                flag = true;
            }

            return predicate;
        }

        public int Getcount(String Searchtoken)
        {
            Expression<Func<Vehicles, bool>> predicate = SearchOnVehicles(Searchtoken);
            return _objectSet.Where(predicate).Count();
        }

        public bool Delete(int id)
        {
            try
            {
                Remove(GetById(id));
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public override RequestResponse AddUpdate(Vehicles entity)
        {
            Validate(entity);

            if (responseObj.ErrorMessages.Count <= 0)
            {
                if (entity.Id > 0)
                {
                    using (VehicleStatusRepository vStatusRepo = new VehicleStatusRepository())
                    {
                        vStatusRepo.AddUpdate(new VehicleStatus()
                        {
                            AreaId = entity.VehicleStatus.AreaId,
                            BatteryStatus = entity.VehicleStatus.BatteryStatus,
                            InRide = entity.VehicleStatus.InRide,
                            InService = entity.VehicleStatus.InService,
                            LatV = entity.VehicleStatus.LatV,
                            LongV = entity.VehicleStatus.LongV,
                            VehicleId = entity.VehicleStatus.VehicleId,
                            VehicleQR = entity.VehicleStatus.VehicleQR
                        });

                        vStatusRepo.Save();
                    }

                    using (VehicleSpecsRepository vSpecsRepo = new VehicleSpecsRepository())
                    {
                        vSpecsRepo.AddUpdate(new VehicleSpecs()
                        {
                            VehicleId = entity.VehicleSpecs.VehicleId,
                            CategoryId = entity.VehicleSpecs.CategoryId,
                            ModelId = entity.VehicleSpecs.ModelId
                        });

                        vSpecsRepo.Save();
                    }

                    Attach(new Vehicles()
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        PlateNo = entity.PlateNo,
                        HolderId = entity.HolderId,
                        IsActive = entity.IsActive,
                        UniqueId = entity.UniqueId,
                        ImageName = entity.ImageName
                    });
                }
                else
                {
                    // Generate QR
                    entity.UniqueId = RandomNumber(10);
                    entity.ImageName = string.IsNullOrEmpty(entity.ImageName) ? "noimage" : entity.ImageName;
                    entity.VehicleStatus.VehicleQR = entity.UniqueId;
                    Add(entity);
                }
                Save();
                responseObj.Messages.Add("succss", OperationLP.SuccessMsg);
            }
            return responseObj;
        }

        public override Vehicles GetById(int id)
        {
            return _objectSet.Include(x => x.VehicleSpecs.VehiclesModel).Include(x => x.VehicleStatus).FirstOrDefault(x => x.Id == id);
        }

        public RequestResponse GetByIdService(int id)
        {
            responseObj.ReturnedObject = _objectSet.Include(x => x.VehicleSpecs.VehiclesModel).Include(x => x.VehicleStatus).Where(x => x.Id == id).ToList();
            return responseObj;
        }


        public RequestResponse GetAvilable()
        {
            responseObj.ReturnedObject = _objectSet.Include(x=>x.VehicleStatus).Where(x => x.VehicleStatus.InRide != true && x.VehicleStatus.InService != true && x.IsActive != false).Select(x => new AvilableVehiclesModel()
            {
                id = x.Id,
                Lat = x.VehicleStatus.LatV,
                Lng = x.VehicleStatus.LongV,
                Name = x.Name,
                iconImageEnum = "Moto",
                PlateNo = x.PlateNo,
                BatteryStatus = x.VehicleStatus.BatteryStatus.ToString(),
                QRCode = x.UniqueId
            }).ToList();

            responseObj.Messages.Add("k1", "Okk");
            return responseObj;
        }

        public override void Validate(Vehicles entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                responseObj.ErrorMessages.Add("InvalidName", "Vehicles name required");
            }

            if (string.IsNullOrEmpty(entity.PlateNo))
            {
                responseObj.ErrorMessages.Add("InvalidPlate", "Plate number required");
            }

            if (string.IsNullOrEmpty(entity.VehicleStatus?.LatV) || string.IsNullOrEmpty(entity.VehicleStatus?.LongV))
            {
                responseObj.ErrorMessages.Add("InvalidLocation", "Vehicles location required");
            }

            if (entity.VehicleSpecs?.CategoryId <= 0 || entity.VehicleSpecs?.CategoryId == null)
            {
                responseObj.ErrorMessages.Add("InvalidCategory", "Vehicles category required");
            }

            if (entity.VehicleSpecs?.ModelId <= 0 || entity.VehicleSpecs?.ModelId == null)
            {
                responseObj.ErrorMessages.Add("InvalidModel", "Vehicles model required");
            }

            if (entity.VehicleStatus?.AreaId <= 0 || entity.VehicleStatus?.AreaId == null)
            {
                responseObj.ErrorMessages.Add("InvalidArea", "Area required");
            }
        }

        public bool IsAvilableByBarcode(string qrNumber, int vId)
        {
            if (_objectSet.Count(x => x.UniqueId == qrNumber && x.Id == vId && x.VehicleStatus.InRide != true) > 0)
            {
                return true;
            }

            return false;
        }
    }

    public class TempRepository : MainRepository<TempStatus>
    {
        public override RequestResponse AddUpdate(TempStatus entity)
        {
            Add(entity);
            Save();
            responseObj.ReturnedObject = entity;
            responseObj.ResponseIdStr = entity.DataStr;
            return responseObj;
        }

        public override TempStatus GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(TempStatus entity)
        {

        }
    }
}
