using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Represents an abstract interceptor that performs additional work with outgoing requests/incoming responses.
    /// </summary>
    public abstract class MessageInterceptor
    {
        /// <summary>
        /// Called by the <see cref="JSendClient"/> before a <see cref="HttpRequestMessage"/> is sent.
        /// </summary>
        /// <param name="request">A request that is about to be dispatched.</param>
        public virtual void OnSending(HttpRequestMessage request)
        {
        }

        /// <summary>
        /// Called by the <see cref="JSendClient"/> when a <see cref="HttpResponseMessage"/> is received.
        /// </summary>
        /// <param name="context">The contextual data associated with the HTTP response message.</param>
        public virtual void OnReceived(ResponseReceivedContext context)
        {
        }

        /// <summary>
        /// Called by the <see cref="JSendClient"/> when a <see cref="HttpResponseMessage"/> is
        /// parsed into a <see cref="JSendResponse{T}"/>.
        /// </summary>
        /// <typeparam name="TResponse">The type of the data contained by the JSend response.</typeparam>
        /// <param name="context">The contextual data associated with the parsing.</param>
        public virtual void OnParsed<TResponse>(ResponseParsedContext<TResponse> context)
        {
        }

        /// <summary>
        /// Called by the <see cref="JSendClient"/> when an exception is caught.
        /// </summary>
        /// <param name="context">The contextual data associated the exception.</param>
        public virtual void OnException(ExceptionContext context)
        {
        }
    }
}
