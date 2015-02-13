using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public abstract class BaseJSendResponse<T>
    {
        private readonly string _status;
        private readonly T _data;

        protected BaseJSendResponse(string status, T data)
        {
            if (status == null) throw new ArgumentNullException("status");
            if (data == null) throw new ArgumentNullException("data");
            _status = status;
            _data = data;
        }

        protected BaseJSendResponse(string status)
        {
            if (status == null) throw new ArgumentNullException("status");
            _status = status;
            _data = default(T);
        }

        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return _status; }
        }

        [JsonProperty("data", Order = 2, NullValueHandling = NullValueHandling.Include)]
        public T Data
        {
            get { return _data; }
        }
    }
}
