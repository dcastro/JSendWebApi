using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    /// <summary>A JSend fail response.</summary>
    public class FailResponse : IJSendResponse
    {
        private readonly object _data;

        /// <summary>Initializes a new instance of <see cref="FailResponse"/>.</summary>
        /// <param name="data">A wrapper for the details of why the request failed.</param>
        public FailResponse(object data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        /// <summary>Gets the status of this response, always set to "fail".</summary>
        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "fail"; }
        }

        /// <summary>Gets the wrapper for the details of why the request failed.</summary>
        [JsonProperty("data", Order = 2)]
        public object Data
        {
            get { return _data; }
        }
    }
}
