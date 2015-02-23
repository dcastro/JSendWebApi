using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class FailResponse
    {
        private readonly object _data;

        public FailResponse(object data)
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
        public object Data
        {
            get { return _data; }
        }
    }
}
