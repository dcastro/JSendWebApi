using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class FailJSendResponse<T> : BaseJSendResponse<T>
    {
        public FailJSendResponse(T data)
            : base("fail", data)
        {
        }
    }
}
