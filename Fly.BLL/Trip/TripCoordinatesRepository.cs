using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class TripCoordinatesRepository : MainRepository<TripCoordinates>
    {
        public override RequestResponse AddUpdate(TripCoordinates entity)
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

        public override TripCoordinates GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public override void Validate(TripCoordinates entity)
        {
            
        }
    }
}
