using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel
{
    public class RegisterModel
    {
        public string device_unique_id { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool gender { get; set; }
        public DateTime date_of_birth { get; set; }
    }

    public class VerifyPassCodeModel
    {
        public string mobileNumber { get; set; }
        public string passCode { get; set; }
    }
}
