using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel;
using System.Data.Entity;
using Fly.DomainModel.Helper;

namespace Fly.BLL
{
    public class SecurityUserRepository : MainRepository<SecurityUser>
    {
        public override SecurityUser GetById(int id)
        {
            throw new NotImplementedException();
        }

        public SecurityUser GetBy(string userName,string password)
        {
            return _objectSet.Include(x=>x.SecurityUserRole.Select(s=>s.SecurityRole)).FirstOrDefault(x=> x.Password == password && (x.Email == password || x.Telephone == password));
        }

        public override RequestResponse AddUpdate(SecurityUser model)
        {
            Validate(model);
            if (responseObj.Messages.Count > 0)
            {
                responseObj.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return responseObj;
            }
            try
            {
                if (model.Id > 0)
                {
                    Attach(model);
                }
                else
                {
                    Add(model);
                }
                
                Save();

                responseObj.StatusCode = System.Net.HttpStatusCode.OK;
                responseObj.IsDone = true;
            }
            catch (Exception ex)
            {
                responseObj.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                responseObj.IsDone = false;
                responseObj.Messages.Add("fillAlldata", ex.InnerException.Message);
            }

            return responseObj;
        }

        public override void Validate(SecurityUser entity)
        {
            // Required
            if (string.IsNullOrEmpty(entity.Email) || string.IsNullOrEmpty(entity.Telephone) || string.IsNullOrEmpty(entity.Password))
            {
                responseObj.Messages.Add("fillAlldata", "Fill all data");
            }

            //Dublicate
            if (_objectSet.Any(x =>x.Id != entity.Id && (x.Email == entity.Email || x.Telephone == entity.Telephone)))
            {
                responseObj.Messages.Add("fillAlldata", "Email || Telephone already exist");
            }
        }

    }
}
