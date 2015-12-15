using System;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Represents a HTTP response message and the contextual data associated with it when it was received.
    /// </summary>
    public class ResponseReceivedContext
    {
        /// <summary>Initializes a new instance of <see cref="ResponseReceivedContext"/>.</summary>
        /// <param name="httpRequest">The HTTP request message that was sent.</param>
        /// <param name="httpResponse">The HTTP response message that was received.</param>
        public ResponseReceivedContext(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse)
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));
            if (httpResponse == null) throw new ArgumentNullException(nameof(httpResponse));

            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
        }

        /// <summary>Gets the HTTP request message that was sent.</summary>
        public HttpRequestMessage HttpRequest { get; }

        /// <summary>Gets the HTTP response message that was received.</summary>
        public HttpResponseMessage HttpResponse { get; }
    }
}
