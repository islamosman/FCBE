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
    public class PromoCodeRepository : MainRepository<PromoCode>
    {
        public override RequestResponse AddUpdate(PromoCode entity)
        {
            Validate(entity);

            if (responseObj.ErrorMessages.Count <= 0)
            {
                if (entity.Id > 0)
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

        public override PromoCode GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public  PromoCode GetByName(string name)
        {
            return _objectSet.FirstOrDefault(x => x.Name == name);
        }

        public List<PromoCode> get(int skip, int take, String Searchtoken, string column, string sortColumnDir)
        {
            if (string.IsNullOrEmpty(Searchtoken))
            {
                return _objectSet.OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take).ToList();
            }
            else
            {
                return _objectSet.Where(x => x.Name.Contains(Searchtoken)).OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take).ToList();
            }


        }

        public int Getcount(String Searchtoken)
        {
            return _objectSet.Where(x => x.Name.Contains(Searchtoken)).Count();
        }

        public override void Validate(PromoCode entity)
        {
            if (string.IsNullOrEmpty(entity.Name) || entity.Percentage <= 0)
            {
                responseObj.ErrorMessages.Add("dataInvalid", "Name, Percentage required!");
            }
        }
    }
}
