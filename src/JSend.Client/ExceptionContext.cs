using System;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// RepresentsRepresents an exception and the contextual data associated with it when it was caught.
    /// </summary>
    public class ExceptionContext
    {
        /// <summary>Gets the request being processed when the exception was caught.</summary>
        public HttpRequestMessage HttpRequest { get; }

        /// <summary>Gets the exception that was caught while processing <see cref="HttpRequest"/>.</summary>
        public Exception Exception { get; }

        /// <summary>Initializes a new instance of <see cref="ExceptionContext"/>.</summary>
        /// <param name="httpRequest">The request being processed when the exception was caught.</param>
        /// <param name="exception">The exception that was caught while processing <paramref name="httpRequest"/>.</param>
        public ExceptionContext(HttpRequestMessage httpRequest, Exception exception)
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            HttpRequest = httpRequest;
            Exception = exception;
        }
    }
}
