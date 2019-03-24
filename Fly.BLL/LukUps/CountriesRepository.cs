using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class CountriesRepository : MainRepository<Countries>
    {
        public override RequestResponse AddUpdate(Countries entity)
        {
            throw new NotImplementedException();
        }

        public override Countries GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Countries entity)
        {
            throw new NotImplementedException();
        }
    }
}
