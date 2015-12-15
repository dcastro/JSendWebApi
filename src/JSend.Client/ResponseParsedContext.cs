using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Represents a <see cref="JSend.Client.JSendResponse{T}"/> and the contextual data associated with it when it was parsed.
    /// </summary>
    /// <typeparam name="TResponse">The type of the data contained by the JSend response.</typeparam>
    public class ResponseParsedContext<TResponse>
    {
        /// <summary>Initializes a new instance of <see cref="ResponseParsedContext{TResponse}"/>.</summary>
        /// <param name="httpRequest">The HTTP request message that was sent.</param>
        /// <param name="httpResponse">The HTTP response message that was received.</param>
        /// <param name="jsendResponse">The parsed JSend response.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "jsend")]
        public ResponseParsedContext(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse,
            JSendResponse<TResponse> jsendResponse)
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));
            if (httpResponse == null) throw new ArgumentNullException(nameof(httpResponse));
            if (jsendResponse == null) throw new ArgumentNullException(nameof(jsendResponse));

            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
            JSendResponse = jsendResponse;
        }

        /// <summary>Gets the HTTP request message that was sent.</summary>
        public HttpRequestMessage HttpRequest { get; }

        /// <summary>Gets the HTTP response message that was received.</summary>
        public HttpResponseMessage HttpResponse { get; }

        /// <summary>Gets the parsed JSend response.</summary>
        public JSendResponse<TResponse> JSendResponse { get; }
    }
}
