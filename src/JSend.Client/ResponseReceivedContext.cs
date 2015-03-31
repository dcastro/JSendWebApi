using System;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Represents a HTTP response message and the contextual data associated with it when it was received.
    /// </summary>
    public class ResponseReceivedContext
    {
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly HttpResponseMessage _httpResponseMessage;

        /// <summary>Initializes a new instance of <see cref="ResponseReceivedContext"/>.</summary>
        /// <param name="httpRequestMessage">The HTTP request message that was sent.</param>
        /// <param name="httpResponseMessage">The HTTP response message that was received.</param>
        public ResponseReceivedContext(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException("httpRequestMessage");
            if (httpResponseMessage == null) throw new ArgumentNullException("httpResponseMessage");

            _httpRequestMessage = httpRequestMessage;
            _httpResponseMessage = httpResponseMessage;
        }

        /// <summary>Gets the HTTP request message that was sent.</summary>
        public HttpRequestMessage HttpRequestMessage
        {
            get { return _httpRequestMessage; }
        }

        /// <summary>Gets the HTTP response message that was received.</summary>
        public HttpResponseMessage HttpResponseMessage
        {
            get { return _httpResponseMessage; }
        }
    }
}
