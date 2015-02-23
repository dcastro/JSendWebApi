using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Responses
{
    public class ErrorResponse
    {
        private readonly string _message;
        private readonly int? _code;
        private readonly object _data;

        public ErrorResponse(string message)
            : this(message, null, null)
        {

        }

        public ErrorResponse(string message, int? code)
            : this(message, code, null)
        {

        }

        public ErrorResponse(string message, object data)
            : this(message, null, data)
        {

        }

        public ErrorResponse(string message, int? code, object data)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be an empty string.", "message");

            _message = message;
            _code = code;
            _data = data;
        }

        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "error"; }
        }

        [JsonProperty("message", Order = 2)]
        public string Message
        {
            get { return _message; }
        }

        [JsonProperty("code", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        public int? Code
        {
            get { return _code; }
        }

        [JsonProperty("data", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
        public object Data
        {
            get { return _data; }
        }
    }
}
