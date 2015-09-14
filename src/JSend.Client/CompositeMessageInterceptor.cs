using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>
    /// Performs additional work with outgoing requests/incoming responses by invoking all
    /// contained <see cref="Interceptors"/>.
    /// </summary>
    public class CompositeMessageInterceptor : MessageInterceptor
    {
        /// <summary>Gets the interceptors contained within this instance.</summary>
        public IEnumerable<MessageInterceptor> Interceptors { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeMessageInterceptor"/>.
        /// </summary>
        /// <param name="interceptors">The message interceptors.</param>
        public CompositeMessageInterceptor(IEnumerable<MessageInterceptor> interceptors)
            : this(interceptors.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeMessageInterceptor"/>.
        /// </summary>
        /// <param name="interceptors">The message interceptors.</param>
        public CompositeMessageInterceptor(params MessageInterceptor[] interceptors)
        {
            if (interceptors == null) throw new ArgumentNullException(nameof(interceptors));

            Interceptors = interceptors;
        }

        /// <summary>
        /// Invokes <see cref="MessageInterceptor.OnSending"/> on the contained <see cref="Interceptors"/>.
        /// </summary>
        /// <param name="request">A request that is about to be dispatched.</param>
        public override void OnSending(HttpRequestMessage request)
        {
            foreach (var interceptor in Interceptors)
                interceptor.OnSending(request);
        }

        /// <summary>
        /// Invokes <see cref="MessageInterceptor.OnReceived"/> on the contained <see cref="Interceptors"/>.
        /// </summary>
        /// <param name="context">The contextual data associated with the HTTP response message.</param>
        public override void OnReceived(ResponseReceivedContext context)
        {
            foreach (var interceptor in Interceptors)
                interceptor.OnReceived(context);
        }

        /// <summary>
        /// Invokes <see cref="MessageInterceptor.OnParsed{TResponse}"/> on the contained <see cref="Interceptors"/>.
        /// </summary>
        /// <typeparam name="TResponse">The type of the data contained by the JSend response.</typeparam>
        /// <param name="context">The contextual data associated with the parsing.</param>
        public override void OnParsed<TResponse>(ResponseParsedContext<TResponse> context)
        {
            foreach (var interceptor in Interceptors)
                interceptor.OnParsed(context);
        }

        /// <summary>
        /// Invokes <see cref="MessageInterceptor.OnException"/> on the contained <see cref="Interceptors"/>.
        /// </summary>
        /// <param name="context">The contextual data associated the exception.</param>
        public override void OnException(ExceptionContext context)
        {
            foreach (var interceptor in Interceptors)
                interceptor.OnException(context);
        }
    }
}
