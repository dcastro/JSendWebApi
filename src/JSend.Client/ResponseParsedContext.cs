using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Represents a <see cref="JSendResponse{T}"/> and the contextual data associated with it when it was parsed.
    /// </summary>
    /// <typeparam name="TResponse">The type of the data contained by the JSend response.</typeparam>
    public class ResponseParsedContext<TResponse>
    {
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly HttpResponseMessage _httpResponseMessage;
        private readonly JSendResponse<TResponse> _jsendResponse;

        /// <summary>Initializes a new instance of <see cref="ResponseParsedContext{TResponse}"/>.</summary>
        /// <param name="httpRequestMessage">The HTTP request message that was sent.</param>
        /// <param name="httpResponseMessage">The HTTP response message that was received.</param>
        /// <param name="jsendResponse">The parsed JSend response.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "jsend")]
        public ResponseParsedContext(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage,
            JSendResponse<TResponse> jsendResponse)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException("httpRequestMessage");
            if (httpResponseMessage == null) throw new ArgumentNullException("httpResponseMessage");
            if (jsendResponse == null) throw new ArgumentNullException("jsendResponse");

            _httpRequestMessage = httpRequestMessage;
            _httpResponseMessage = httpResponseMessage;
            _jsendResponse = jsendResponse;
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

        /// <summary>Gets the parsed JSend response.</summary>
        public JSendResponse<TResponse> JSendResponse
        {
            get { return _jsendResponse; }
        }
    }
}
