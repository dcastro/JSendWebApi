using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSendWebApi;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class SuccessJSendResponse<T> : SuccessJSendResponse
    {
        private readonly T _data;

        public SuccessJSendResponse(T data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        [JsonProperty("data", Order = 2)]
        public T Data
        {
            get { return _data; }
        }
    }
}
