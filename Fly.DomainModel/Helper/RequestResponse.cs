using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel.Helper
{
 public   class RequestResponse
    {
        public RequestResponse()
        {
            Messages = new Dictionary<string, string>();
        }
        public bool IsDone { get; set; }
        public long ResponseId { get; set; }
        public string ResponseMessege { get; set; }
        public Dictionary<string, string> Messages { get; set; }
        public object ReturnedObject { set; get; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
