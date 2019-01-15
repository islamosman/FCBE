using Fly.DomainModel.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.BLL
{
    public class BaseBussiness
    {
        protected RequestResponse responseObj;
        public BaseBussiness()
        {
            responseObj = new RequestResponse() { /*IsDone = false */};
        }

        public string RandomNumber(int length)
        {
            Random randomObj = new Random();
            return Guid.NewGuid().ToString().Substring(0, (length / 2 - 1)).ToString() + randomObj.Next(length / 2, length / 2).ToString();
        }
    }
}
