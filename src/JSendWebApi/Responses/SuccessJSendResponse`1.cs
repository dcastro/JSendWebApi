using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSendWebApi;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class SuccessJSendResponse<T>
    {
        private readonly T _data;

        public SuccessJSendResponse(T data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "success"; }
        }

        [JsonProperty("data", Order = 2)]
        public T Data
        {
            get { return _data; }
        }
    }
}
