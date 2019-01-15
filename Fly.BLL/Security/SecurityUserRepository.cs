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

        public SecurityUser GetBy(string userName, string password)
        {
            return _objectSet.Include(x => x.SecurityUserRole.Select(s => s.SecurityRole)).FirstOrDefault(x => x.Password == password && (x.Email == password || x.Telephone == password));
        }

        public override RequestResponse AddUpdate(SecurityUser model)
        {
            Validate(model);
            if (responseObj.ErrorMessages.Count > 0)
            {
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
                    model.PassCode = RandomNumber(10);
                    model.SecurityUserRole.Add(new SecurityUserRole()
                    {
                        RoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UserRole"].ToString())
                    });

                    Add(model);
                }

                Save();
                responseObj.Messages.Add("success", "Welcoom,Registerd Successfully. PLease verfiy the code.");
            }
            catch (Exception ex)
            {
                responseObj.ErrorMessages.Add("fillAlldata", ex.InnerException.Message);
            }

            return responseObj;
        }

        public RequestResponse VerfiyPassCode(VerifyPassCodeModel verifyModel)
        {
            SecurityUser secUserModel = _objectSet.FirstOrDefault(x => x.PassCode == verifyModel.passCode && x.Telephone == verifyModel.mobileNumber);
            if (secUserModel != null)
            {
                secUserModel.IsActive = true;
                Attach(secUserModel);
                Save();
            }
            else
            {
                responseObj.ErrorMessages.Add("NotValid", "Invalid Verification Code!");
            }

            return responseObj;
        }

        public override void Validate(SecurityUser entity)
        {
            // Required
            if (string.IsNullOrEmpty(entity.Email) || string.IsNullOrEmpty(entity.Telephone) || string.IsNullOrEmpty(entity.Password))
            {
                responseObj.ErrorMessages.Add("fillAlldata", "Fill all data");
            }

            //Dublicate
            if (_objectSet.Any(x => x.Email == entity.Email || x.Telephone == entity.Telephone))
            {
                responseObj.ErrorMessages.Add("fillAlldata", "Email || Telephone already exist");
            }
        }

    }
}
