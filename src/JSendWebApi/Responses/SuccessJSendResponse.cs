using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class SuccessJSendResponse : BaseJSendResponse<object>
    {
        public SuccessJSendResponse()
            : base("success")
        {
        }
    }
}
