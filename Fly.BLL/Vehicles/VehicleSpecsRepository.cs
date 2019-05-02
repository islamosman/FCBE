using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.DomainModel;
using Fly.Resources;

namespace Fly.BLL
{
    public class VehicleSpecsRepository : MainRepository<VehicleSpecs>
    {
        public override RequestResponse AddUpdate(VehicleSpecs entity)
        {
            Validate(entity);

            if (responseObj.ErrorMessages.Count <= 0)
            {
                if (entity.VehicleId > 0)
                {
                    Attach(entity);
                }
                else
                {
                    Add(entity);
                }
                Save();
                responseObj.Messages.Add("succss", OperationLP.SuccessMsg);
            }
            return responseObj;
        }

        public override VehicleSpecs GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.VehicleId == id);
        }

        public override void Validate(VehicleSpecs entity)
        {
        }
    }
}
