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
                .Include(x => x.VehicleSpecs).Include(x => x.VehicleStatus).OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take)
                  .ToList();
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
                    Attach(entity);
                }
                else
                {
                    // Generate QR
                    entity.UniqueId = "ddd";
                    entity.ImageName= "ddd";
                    Add(entity);
                }
                Save();
                responseObj.Messages.Add("succss", OperationLP.SuccessMsg);
            }
            return responseObj;
        }

        public override Vehicles GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public RequestResponse GetAvilable()
        {
            responseObj.ReturnedObject = _objectSet.Where(x => x.VehicleStatus.InRide != true && x.IsActive != false).Select(x => new AvilableVehiclesModel()
            {
                id = x.Id,
                Lat = x.VehicleStatus.LatV,
                Lng = x.VehicleStatus.LongV,
                Name = x.Name,
                iconImageEnum = "Moto",
                description = x.PlateNo
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
            throw new NotImplementedException();
        }
    }
}
