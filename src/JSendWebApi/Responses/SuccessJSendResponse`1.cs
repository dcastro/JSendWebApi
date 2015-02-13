using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSendWebApi;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class SuccessJSendResponse<T> : BaseJSendResponse<T>
    {
        public SuccessJSendResponse(T data)
            : base("success", data)
        {
        }
    }
}
