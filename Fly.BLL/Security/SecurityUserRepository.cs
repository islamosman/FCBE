using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel;
using System.Data.Entity;
using Fly.DomainModel.Helper;
using System.Linq.Expressions;

namespace Fly.BLL
{
    public class SecurityUserRepository : MainRepository<SecurityUser>
    {
        public override SecurityUser GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public SecurityUser GetBy(string userName, string password)
        {
            int userRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UserRole"]);
            return _objectSet.Include(x => x.SecurityUserRole.Select(s => s.SecurityRole)).FirstOrDefault(x => x.Password == password && x.SecurityUserRole.Any(s => s.RoleId == userRoleId) && (x.Email == userName || x.Telephone == userName));
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
                    model.PayMobSendId = RandomNumber(11);
                    model.PassCode = RandomNumber(10);
                    model.SecurityUserRole.Add(new SecurityUserRole()
                    {
                        RoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UserRole"].ToString())
                    });

                    Add(model);
                }

                Save();
                responseObj.ResponseIdStr = model.PassCode;
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
            SecurityUser secUserModel = _objectSet.FirstOrDefault(x => x.PassCode == verifyModel.passCode);//&& x.Telephone == verifyModel.mobileNumber
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
            if (_objectSet.Any(x => (x.Email == entity.Email || x.Telephone == entity.Telephone) && x.Id != entity.Id))
            {
                responseObj.ErrorMessages.Add("fillAlldata", "Email || Telephone already exist");
            }
        }

        public SecurityUser GetUser(string email)
        {
            int userRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRole"]);
            return _objectSet.Include(x => x.SecurityUserRole).FirstOrDefault(x => x.Email == email);
        }
        public SecurityUser GetUser(string email, string password)
        {
            int userRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRole"]);
            return _objectSet.Include(x => x.SecurityUserRole).FirstOrDefault(x => x.Email == email && x.Password == password && x.SecurityUserRole.Any(s => s.RoleId == userRoleId));
        }


        public List<SecurityUser> get(int skip, int take, String Searchtoken, string column, string sortColumnDir)
        {
            if (column == "Name")
            {
                column = "FullName";
            }

            Expression<Func<SecurityUser, bool>> predicate = SearchOnUsers(Searchtoken);

            return _objectSet.Where(predicate)
                .OrderBy(column + " " + sortColumnDir).Skip(skip).Take(take)
                  .ToList();
        }


        public Expression<Func<SecurityUser, bool>> SearchOnUsers(String Searchtoken)
        {
            int roleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UserRole"].ToString());
            bool flag = false;
            Expression<Func<SecurityUser, bool>> predicate;
            predicate = PredicateBuilder.True<SecurityUser>();

            predicate = predicate.And(i => i.SecurityUserRole.Any(x => x.RoleId == roleId));
            //  flag = true;
            if (!string.IsNullOrEmpty(Searchtoken))
            {
                predicate = predicate.And(s => s.FullName.Contains(Searchtoken));
                flag = true;
            }

            return predicate;
        }

        public int Getcount(String Searchtoken)
        {
            Expression<Func<SecurityUser, bool>> predicate = SearchOnUsers(Searchtoken);
            return _objectSet.Where(predicate).Count();
        }

        public RequestResponse GetStatus(int userId)
        {
            SecurityUser currentUser = GetById(userId);

            if (currentUser.IsPaied != true)
            {
                currentUser.PayMobSendId = RandomNumber(11);
                AddUpdate(currentUser);
                responseObj = new RequestResponse();
            }

            responseObj.ReturnedObject = new
            {
                IdStatus = string.IsNullOrEmpty(currentUser.IdString) ? false : true,
                VisaStatus = currentUser.IsPaied == true ? true : false,
                RefundOrderId = currentUser.RefundPayMobId,
                Tocken = currentUser.TockenToP,
                UserId = currentUser.PayMobSendId,
                IsRefunded = currentUser.IsRefunded
            };

            return responseObj;
        }

        public RequestResponse UpdatePayment(string userPaumentId, string orderId)
        {
            var currentUser = _objectSet.FirstOrDefault(x => x.PayMobSendId == userPaumentId);
            if (currentUser != null)
            {
                currentUser.PayMobId = orderId;
                AddUpdate(currentUser);
            }
            else
            {
                responseObj.ErrorMessages.Add("invalidd", "Invalid Data");
            }

            return responseObj;
        }

        public RequestResponse UpdatePaymentDone(string tocken, string orderId)
        {
            var currentUser = _objectSet.FirstOrDefault(x => x.PayMobId == orderId);
            if (currentUser != null)
            {
                currentUser.TockenToP = tocken;
                currentUser.IsPaied = true;
                AddUpdate(currentUser);
            }
            else
            {
                responseObj.ErrorMessages.Add("invalidd", "Invalid Data");
            }

            return responseObj;
        }


        public RequestResponse UpdateRefundOrderId(string transactionId, string orderId)
        {
            var currentUser = _objectSet.FirstOrDefault(x => x.PayMobId == orderId);
            if (currentUser != null)
            {
                currentUser.RefundPayMobId = transactionId;
                AddUpdate(currentUser);
            }
            else
            {
                responseObj.ErrorMessages.Add("invalidd", "Invalid Data");
            }

            return responseObj;
        }


        public RequestResponse UpdateRefundDone(int userId)
        {
            var currentUser = _objectSet.FirstOrDefault(x => x.Id == userId);
            if (currentUser != null)
            {
                currentUser.IsRefunded = true;
                AddUpdate(currentUser);
            }
            else
            {
                responseObj.ErrorMessages.Add("invalidd", "Invalid Data");
            }

            return responseObj;
        }

    }
}
