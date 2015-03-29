using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSend.Client
{
    /// <summary>
    /// Sends HTTP requests and parses JSend-formatted HTTP responses.
    /// </summary>
    public class JSendClient : IJSendClient
    {
        private readonly IJSendParser _parser;
        private readonly Encoding _encoding;
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly Func<HttpClient> _clientFactory;

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        public JSendClient()
            : this(new JSendClientSettings())
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="settings">The settings to configure this client.</param>
        public JSendClient(JSendClientSettings settings)
            : this(settings, () => new HttpClient())
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="settings">The settings to configure this client.</param>
        /// <param name="clientFactory">A factory that creates instances of <see cref="HttpClient"/>.</param>
        public JSendClient(JSendClientSettings settings, Func<HttpClient> clientFactory)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (clientFactory == null) throw new ArgumentNullException("clientFactory");

            _parser = settings.Parser ?? new DefaultJSendParser();
            _encoding = settings.Encoding;
            _serializerSettings = settings.SerializerSettings;

            _clientFactory = clientFactory;
        }

        /// <summary>Gets the parser used to process JSend-formatted responses.</summary>
        public IJSendParser Parser
        {
            get { return _parser; }
        }

        /// <summary>Gets the encoding used to format a request's content.</summary>
        public Encoding Encoding
        {
            get { return _encoding; }
        }

        /// <summary>Gets the settings used to serialize the content of a request.</summary>
        public JsonSerializerSettings SerializerSettings
        {
            get { return _serializerSettings; }
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
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient will dispose of the request object.")]
        public Task<JSendResponse<T>> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return SendAsync<T>(request, cancellationToken);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<TResponse>> PostAsync<TResponse>(string requestUri, object content)
        {
            return PostAsync<TResponse>(new Uri(requestUri), content);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content)
        {
            return PostAsync<TResponse>(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient will dispose of the request object.")]
        public Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken)
        {
            var serialized = JsonConvert.SerializeObject(content, _serializerSettings);

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(serialized, _encoding, "application/json")
            };

            return SendAsync<TResponse>(request, cancellationToken);
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task<JSendResponse<T>> SendAsync<T>(HttpRequestMessage request)
        {
            return SendAsync<T>(request, CancellationToken.None);
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<JSendResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var client = _clientFactory())
            {
                var responseMessage = await client.SendAsync(request, cancellationToken);
                return await _parser.ParseAsync<T>(responseMessage);
            }
        }
    }
}
