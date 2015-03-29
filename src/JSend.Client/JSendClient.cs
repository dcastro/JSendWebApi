using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JSend.Client
{
    /// <summary>
    /// Sends HTTP requests and parses JSend-formatted HTTP responses.
    /// </summary>
    public class JSendClient : IJSendClient
    {
        private readonly IJSendParser _parser;
        private readonly Func<HttpClient> _clientFactory;

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        public JSendClient()
            : this(new DefaultJSendParser())
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="parser">A parser to process JSend-formatted responses.</param>
        public JSendClient(IJSendParser parser)
            : this(parser, () => new HttpClient())
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="parser">A parser to process JSend-formatted responses.</param>
        /// <param name="clientFactory">A factory that creates instances of <see cref="HttpClient"/>.</param>
        public JSendClient(IJSendParser parser, Func<HttpClient> clientFactory)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (clientFactory == null) throw new ArgumentNullException("clientFactory");

            _parser = parser;
            _clientFactory = clientFactory;
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<T>> GetAsync<T>(string requestUri)
        {
            return GetAsync<T>(new Uri(requestUri));
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task<JSendResponse<T>> GetAsync<T>(Uri requestUri)
        {
            return GetAsync<T>(requestUri, CancellationToken.None);
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<JSendResponse<T>> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            using (var client = _clientFactory())
            {
                var responseMessage = await client.GetAsync(requestUri, cancellationToken);
                return await _parser.ParseAsync<T>(responseMessage);
            }
        }
    }
}
