using System;
using JSend.Client.Properties;
using Newtonsoft.Json.Linq;

namespace JSend.Client
{
    /// <summary>Represents an error returned by a JSend API.</summary>
    public class JSendError
    {
        private readonly JSendStatus _status;
        private readonly string _message;
        private readonly int? _code;
        private readonly JToken _data;

        /// <summary>Initializes a new instance of <see cref="JSendError"/>.</summary>
        /// <param name="status">
        /// The status of the response. Must be either <see cref="JSendStatus.Error"/> or <see cref="JSendStatus.Fail"/>.
        /// </param>
        /// <param name="message">
        /// A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.
        /// </param>
        /// <param name="code">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data">A generic wrapper for the details of why the request failed.</param>
        public JSendError(JSendStatus status, string message, int? code, JToken data)
        {
            if (status == JSendStatus.Success)
                throw new ArgumentException(StringResources.ErrorWithSuccessStatus, "status");

            _status = status;
            _message = message;
            _code = code;
            _data = data;
        }

        /// <summary>Gets the status of this response.</summary>
        public JSendStatus Status
        {
            get { return _status; }
        }

        /// <summary>Gets the error message explaining what went wrong.</summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>Gets the numeric error code corresponding to the error.</summary>
        public int? Code
        {
            get { return _code; }
        }

        /// <summary>Gets the generic data container.</summary>
        public JToken Data
        {
            get { return _data; }
        }
    }
}
