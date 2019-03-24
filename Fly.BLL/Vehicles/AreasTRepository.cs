using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.Resources;
using System.Data.Entity;

namespace Fly.BLL
{
    public class AreasTRepository : MainRepository<AreasT>
    {
        public override RequestResponse AddUpdate(AreasT entity)
        {
            Validate(entity);

            if (responseObj.ErrorMessages.Count <= 0)
            {
                if (entity.Id > 0 )
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

        public override AreasT GetById(int id)
        {
            return _objectSet.Include(x => x.Countries).FirstOrDefault(x => x.Id == id);
        }

        public override void Validate(AreasT entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                responseObj.ErrorMessages.Add("NameR", "Name required");
            }
            if (string.IsNullOrEmpty(entity.AreaCoordinates))
            {
                responseObj.ErrorMessages.Add("CordR", "Area required");
            }
        }

        public List<AreasT> get(int skip, int take, String Searchtoken, string column, string sortColumnDir)
        {
            if (string.IsNullOrEmpty(Searchtoken))
            {
                return _objectSet.Include(x=>x.Countries).OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take).ToList();
            }
            else
            {
                return _objectSet.Include(x => x.Countries).Where(x => x.Name.Contains(Searchtoken)).OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take).ToList();
            }


        }

        public int Getcount(String Searchtoken)
        {
            return _objectSet.Where(x => x.Name.Contains(Searchtoken)).Count();
        }

        public bool Delete(int id)
        {
            try
            {
                Remove(GetById(id));
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
