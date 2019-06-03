using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel;
using Fly.DomainModel.Helper;
using Fly.Resources;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Fly.BLL
{
    public class SubscriptionRepository : MainRepository<SubscriptionV>
    {
        public override RequestResponse AddUpdate(SubscriptionV entity)
        {
            Validate(entity);


            if (responseObj.ErrorMessages.Count <= 0)
            {
                switch (entity.DaysCount)
                {
                    case 1:
                        entity.PayAmount = 80;
                        break;
                    case 3:
                        entity.PayAmount = 220;
                        break;
                    case 7:
                        entity.PayAmount = 470;
                        break;
                    case 30:
                        entity.PayAmount = 1350;
                        break;
                    default:
                        entity.PayAmount = 0;
                        break;
                }

                if (entity.PromoCodeId > 0 && entity.DaysCount != 30)
                {
                    decimal promoDiscount = 0;
                    using (PromoCodeRepository promoRepo = new PromoCodeRepository())
                    {
                        PromoCode promoModel = promoRepo.GetById(entity.PromoCodeId.Value);
                        if (promoModel != null)
                        {
                            promoDiscount = promoModel.Percentage;
                        }
                    }
                    entity.PayAmount = entity.PayAmount - ((entity.PayAmount * promoDiscount) / 100);
                }

                if (entity.Id > 0)
                {
                    Attach(entity);
                }
                else
                {
                    Add(entity);
                }

                Save();

                responseObj.ReturnedObject = new
                {
                    id = entity.Id,
                    amount = entity.PayAmount
                };
                responseObj.Messages.Add("succss", OperationLP.SuccessMsg);
            }

            return responseObj;
        }

        public override SubscriptionV GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.Id == id);
        }

        public void DoneById(int id,bool isDone)
        {
            SubscriptionV currentModel= _objectSet.FirstOrDefault(x => x.Id == id);
            currentModel.IsDone = isDone;
            AddUpdate(currentModel);
        }

        public override void Validate(SubscriptionV entity)
        {
            if (string.IsNullOrEmpty(entity.Name) || string.IsNullOrEmpty(entity.PhoneNumber.ToString()) || entity.PhoneNumber <= 0 || string.IsNullOrEmpty(entity.LocationStr) || string.IsNullOrEmpty(entity.PickDateTime.ToString()))
            {
                responseObj.ErrorMessages.Add("invalidD", "Invalid Data");
            }
        }


        public List<SubscriptionV> get(int skip, int take, String Searchtoken, string column, string sortColumnDir)
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


        public Expression<Func<SubscriptionV, bool>> SearchOnVehicles(String Searchtoken)
        {
            bool flag = false;
            Expression<Func<SubscriptionV, bool>> predicate;
            predicate = PredicateBuilder.True<SubscriptionV>();

            //predicate = predicate.And(i => i. == isPhoto);
            //  flag = true;
            if (!string.IsNullOrEmpty(Searchtoken))
            {
                predicate = predicate.And(s => s.Name.Contains(Searchtoken));
                flag = true;
            }

            return predicate;
        }

        public int Getcount(String Searchtoken)
        {
            Expression<Func<SubscriptionV, bool>> predicate = SearchOnVehicles(Searchtoken);
            return _objectSet.Where(predicate).Count();
        }
    }
}
