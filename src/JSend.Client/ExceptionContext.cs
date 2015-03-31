using System;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// RepresentsRepresents an exception and the contextual data associated with it when it was caught.
    /// </summary>
    public class ExceptionContext
    {
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly Exception _exception;

        /// <summary>Initializes a new instance of <see cref="ExceptionContext"/>.</summary>
        /// <param name="httpRequestMessage">The request being processed when the exception was caught.</param>
        /// <param name="exception">The exception that was caught while processing <paramref name="httpRequestMessage"/>.</param>
        public ExceptionContext(HttpRequestMessage httpRequestMessage, Exception exception)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException("httpRequestMessage");
            if (exception == null) throw new ArgumentNullException("exception");

            _httpRequestMessage = httpRequestMessage;
            _exception = exception;
        }

        /// <summary>Gets the request being processed when the exception was caught.</summary>
        public HttpRequestMessage HttpRequestMessage
        {
            get { return _httpRequestMessage; }
        }

        /// <summary>Gets the exception that was caught while processing <see cref="HttpRequestMessage"/>.</summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
