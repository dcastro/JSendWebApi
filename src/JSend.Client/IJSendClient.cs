using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSend.Client
{
    /// <summary>
    /// Sends HTTP requests and parses JSend-formatted HTTP responses.
    /// </summary>
    public interface IJSendClient : IDisposable
    {
        /// <summary>Gets the settings used to serialize requests/deserialize responses.</summary>
        JsonSerializerSettings SerializerSettings { get; }

        /// <summary>Gets the client used to send HTTP requests and receive HTTP responses.</summary>
        HttpClient HttpClient { get; }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(string requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri, CancellationToken cancellationToken);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(string requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PostAsync(string requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PostAsync(Uri requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PostAsync(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> DeleteAsync(string requestUri);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> DeleteAsync(Uri requestUri);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> DeleteAsync(Uri requestUri, CancellationToken cancellationToken);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(string requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PutAsync(string requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PutAsync(Uri requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<JToken>> PutAsync(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request);

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="JSendRequestException">An error occurred while sending the request.</exception>
        /// <exception cref="JSendParseException">An error occurred while parsing the response.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
