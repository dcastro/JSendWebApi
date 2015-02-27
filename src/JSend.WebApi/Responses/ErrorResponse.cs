using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSend.WebApi.Properties;
using Newtonsoft.Json;

namespace JSend.WebApi.Responses
{
    /// <summary>
    /// A JSend error response.
    /// </summary>
    public class ErrorResponse : IJSendResponse
    {
        private readonly string _message;
        private readonly int? _code;
        private readonly object _data;

        /// <summary>Initializes a new instance of <see cref="ErrorResponse"/>.</summary>
        /// <param name="message">An error message.</param>
        public ErrorResponse(string message)
            : this(message, null, null)
        {

        }

        /// <summary>Initializes a new instance of <see cref="ErrorResponse"/>.</summary>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="code">A numeric code corresponding to the error, if applicable.</param>
        public ErrorResponse(string message, int? code)
            : this(message, code, null)
        {

        }

        /// <summary>Initializes a new instance of <see cref="ErrorResponse"/>.</summary>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="data">An optional generic container for any other information about the error.</param>
        public ErrorResponse(string message, object data)
            : this(message, null, data)
        {

        }

        /// <summary>Initializes a new instance of <see cref="ErrorResponse"/>.</summary>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="code">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data">An optional generic container for any other information about the error.</param>
        public ErrorResponse(string message, int? code, object data)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(StringResources.ErrorResponse_WhiteSpaceMessage, "message");

            _message = message;
            _code = code;
            _data = data;
        }

        /// <summary>Gets the status of this response, always set to "error".</summary>
        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "error"; }
        }

        /// <summary>Gets the error message explaining what went wrong.</summary>
        [JsonProperty("message", Order = 2)]
        public string Message
        {
            get { return _message; }
        }

        /// <summary>Gets the numeric error code corresponding to the error.</summary>
        [JsonProperty("code", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        public int? Code
        {
            get { return _code; }
        }

        /// <summary>Gets the generic data container; null if not applicable.</summary>
        [JsonProperty("data", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
        public object Data
        {
            get { return _data; }
        }
    }
}
