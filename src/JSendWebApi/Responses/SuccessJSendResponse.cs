using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class SuccessJSendResponse
    {
        private readonly object _data;

        public SuccessJSendResponse()
            : this(null)
        {
        }

        public SuccessJSendResponse(object data)
        {
            _data = data;
        }

        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "success"; }
        }

        [JsonProperty("data", Order = 2, NullValueHandling = NullValueHandling.Include)]
        public object Data
        {
            get { return _data; }
        }
    }
}
