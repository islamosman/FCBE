using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel.Helper
{
    public class RequestResponse
    {
        public RequestResponse()
        {
            Messages = new Dictionary<string, string>();
            ErrorMessages = new Dictionary<string, string>();
        }
        public bool IsDone { get { return ErrorMessages.Count > 0 ? false : true; } }
        public long ResponseId { get; set; }
        public string ResponseIdStr { get; set; }
        public string MessegesStr { get { return FillMessages(Messages); } }
        public string ErrorMessegesStr { get { return FillMessages(ErrorMessages); } }
        public Dictionary<string, string> Messages { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; }
        public object ReturnedObject { set; get; }

        private string FillMessages(Dictionary<string, string> ValidationErrors)
        {
            StringBuilder errorList = new StringBuilder();

            foreach (var item in ValidationErrors)
                errorList.Append(item.Value + ",");

            return errorList.ToString();
        }
    }
}
