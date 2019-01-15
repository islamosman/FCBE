using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;

namespace Fly.BLL.Trip
{
    public class TripRepository : MainRepository<Trips>
    {
        public override RequestResponse AddUpdate(Trips entity)
        {
            throw new NotImplementedException();
        }

        public override Trips GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Trips entity)
        {
            throw new NotImplementedException();
        }
    }
}
