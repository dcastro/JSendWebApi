using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class FailJSendResponse<T>
    {
        private readonly T _data;

        public FailJSendResponse(T data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "fail"; }
        }

        [JsonProperty("data", Order = 2)]
        public T Data
        {
            get { return _data; }
        }
    }
}
