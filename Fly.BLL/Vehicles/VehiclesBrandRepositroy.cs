using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.DomainModel;

namespace Fly.BLL
{
    public class VehiclesBrandRepositroy : MainRepository<VehiclesBrand>
    {
        public override RequestResponse AddUpdate(VehiclesBrand entity)
        {
            throw new NotImplementedException();
        }

        public override VehiclesBrand GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(VehiclesBrand entity)
        {
            throw new NotImplementedException();
        }
    }
}
