using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.DomainModel;

namespace Fly.BLL
{
    public class VehicleStatusRepository : MainRepository<VehicleStatus>
    {
        public override RequestResponse AddUpdate(VehicleStatus entity)
        {
            throw new NotImplementedException();
        }

        public override VehicleStatus GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(VehicleStatus entity)
        {
            throw new NotImplementedException();
        }
    }
}
