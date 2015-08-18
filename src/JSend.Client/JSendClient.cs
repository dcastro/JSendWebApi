using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JSend.Client.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSend.Client
{
    /// <summary>
    /// Sends HTTP requests and parses JSend-formatted HTTP responses.
    /// </summary>
    public class JSendClient : IJSendClient
    {
        private readonly IJSendParser _parser;
        private readonly MessageInterceptor _interceptor;
        private readonly Encoding _encoding;
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly HttpClient _httpClient;

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        public JSendClient()
            : this(null)
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="settings">The settings to configure this client.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient is disposed of as part of this class.")]
        public JSendClient(JSendClientSettings settings)
            : this(settings, new HttpClient())
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendClient"/>.</summary>
        /// <param name="settings">The settings to configure this client.</param>
        /// <param name="httpClient">A client to send HTTP requests and receive HTTP responses.</param>
        public JSendClient(JSendClientSettings settings, HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            if (settings == null)
                settings = new JSendClientSettings();

            _parser = settings.Parser ?? DefaultJSendParser.Instance;
            _interceptor = settings.MessageInterceptor ?? NullMessageInterceptor.Instance;
            _encoding = settings.Encoding;
            _serializerSettings = settings.SerializerSettings;

            _httpClient = httpClient;
        }

        /// <summary>Gets the parser used to process JSend-formatted responses.</summary>
        public IJSendParser Parser
        {
            get { return _parser; }
        }

        /// <summary>Gets the interceptor to perform additional work with outgoing requests/incoming responses.</summary>
        public MessageInterceptor MessageInterceptor
        {
            get { return _interceptor; }
        }

        /// <summary>Gets the encoding used to format a request's content.</summary>
        public Encoding Encoding
        {
            get { return _encoding; }
        }

        /// <summary>Gets the settings used to serialize requests/deserialize responses.</summary>
        public JsonSerializerSettings SerializerSettings
        {
            get { return _serializerSettings; }
        }

        /// <summary>Gets the client used to send HTTP requests and receive HTTP responses.</summary>
        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<TResponse>> GetAsync<TResponse>(string requestUri)
        {
            var uri = BuildUri(requestUri);
            return GetAsync<TResponse>(uri);
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri)
        {
            return GetAsync<TResponse>(requestUri, CancellationToken.None);
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient will dispose of the request object.")]
        public Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return SendAsync<TResponse>(request, cancellationToken);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<TResponse>> PostAsync<TResponse>(string requestUri, object content)
        {
            var uri = BuildUri(requestUri);
            return PostAsync<TResponse>(uri, content);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
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
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient will dispose of the request object.")]
        public Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = Serialize(content)
            };

            return SendAsync<TResponse>(request, cancellationToken);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<JToken>> PostAsync(string requestUri, object content)
        {
            var uri = BuildUri(requestUri);
            return PostAsync(uri, content);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<JToken>> PostAsync(Uri requestUri, object content)
        {
            return PostAsync(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public async Task<JSendResponse<JToken>> PostAsync(Uri requestUri, object content, CancellationToken cancellationToken)
        {
            return await PostAsync<JToken>(requestUri, content, cancellationToken);
        }

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<JToken>> DeleteAsync(string requestUri)
        {
            var uri = BuildUri(requestUri);
            return DeleteAsync(uri);
        }

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<JToken>> DeleteAsync(Uri requestUri)
        {
            return DeleteAsync(requestUri, CancellationToken.None);
        }

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public async Task<JSendResponse<JToken>> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            return await SendAsync<JToken>(request, cancellationToken);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<TResponse>> PutAsync<TResponse>(string requestUri, object content)
        {
            var uri = BuildUri(requestUri);
            return PutAsync<TResponse>(uri, content);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content)
        {
            return PutAsync<TResponse>(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The HttpClient will dispose of the request object.")]
        public Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = Serialize(content)
            };
            return SendAsync<TResponse>(request, cancellationToken);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This is a false positive, see bug report here https://connect.microsoft.com/VisualStudio/feedback/details/1185269")]
        public Task<JSendResponse<JToken>> PutAsync(string requestUri, object content)
        {
            var uri = BuildUri(requestUri);
            return PutAsync(uri, content);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<JToken>> PutAsync(Uri requestUri, object content)
        {
            return PutAsync(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public async Task<JSendResponse<JToken>> PutAsync(Uri requestUri, object content, CancellationToken cancellationToken)
        {
            return await PutAsync<JToken>(requestUri, content, cancellationToken);
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request)
        {
            return SendAsync<TResponse>(request, CancellationToken.None);
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        public async Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                _interceptor.OnSending(request);

                HttpResponseMessage responseMessage;
                try
                {
                    responseMessage = await _httpClient.SendAsync(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _interceptor.OnException(new ExceptionContext(request, ex));
                    throw;
                }
                _interceptor.OnReceived(new ResponseReceivedContext(request, responseMessage));

                JSendResponse<TResponse> jsendResponse;
                try
                {
                    jsendResponse = await _parser.ParseAsync<TResponse>(_serializerSettings, responseMessage);
                }
                catch (Exception ex)
                {
                    _interceptor.OnException(new ExceptionContext(request, ex));
                    throw;
                }
                _interceptor.OnParsed(new ResponseParsedContext<TResponse>(request, responseMessage, jsendResponse));

                return jsendResponse;
            }
            catch (HttpRequestException ex)
            {
                // Wrap HttpClient exceptions in a JSendRequestException.
                // Other exceptions, such as InvalidOperationException, should not be wrapped.
                throw new JSendRequestException(StringResources.HttpClientExecutionError, ex);
            }
        }

        private HttpContent Serialize(object content)
        {
            var serialized = JsonConvert.SerializeObject(content, _serializerSettings);
            return new StringContent(serialized, _encoding, "application/json");
        }

        private static Uri BuildUri(string uri)
        {
            // The URI is allowed to be null if HttpClient.BaseAddress isn't.
            if (string.IsNullOrEmpty(uri))
                return null;

            // The URI is allowed to be relative as long as HttpClient.BaseAddress is not null.
            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // In case a derived class has a custom finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _httpClient.Dispose();
        }
    }
}
