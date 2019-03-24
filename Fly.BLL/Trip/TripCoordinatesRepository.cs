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
            throw new NotImplementedException();
        }

        public override TripCoordinates GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(TripCoordinates entity)
        {
            throw new NotImplementedException();
        }
    }
}
