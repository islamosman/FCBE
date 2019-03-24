using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class VehiclesCategoryRepository : MainRepository<VehiclesCategory>
    {
        public override RequestResponse AddUpdate(VehiclesCategory entity)
        {
            throw new NotImplementedException();
        }

        public override VehiclesCategory GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(VehiclesCategory entity)
        {
            throw new NotImplementedException();
        }
    }
}
