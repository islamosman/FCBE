using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class VehicleRepository : MainRepository<Vehicles>
    {
        public override RequestResponse AddUpdate(Vehicles entity)
        {
            throw new NotImplementedException();
        }

        public override Vehicles GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Vehicles entity)
        {
            throw new NotImplementedException();
        }
    }
}
