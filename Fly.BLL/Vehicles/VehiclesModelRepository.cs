using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class VehiclesModelRepository : MainRepository<VehiclesModel>
    {
        public override RequestResponse AddUpdate(VehiclesModel entity)
        {
            throw new NotImplementedException();
        }

        public override VehiclesModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public override void Validate(VehiclesModel entity)
        {
            throw new NotImplementedException();
        }

        public List<VehiclesModel> GetByBrand(int brandId)
        {
            return _objectSet.Where(x => x.BrandId == brandId).ToList();
        }
    }
}
