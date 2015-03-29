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
    public interface IJSendClient
    {
        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(string requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> GetAsync<TResponse>(Uri requestUri, CancellationToken cancellationToken);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(string requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PostAsync(string requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PostAsync(Uri requestUri, object content);

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PostAsync(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> DeleteAsync(string requestUri);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> DeleteAsync(Uri requestUri);

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> DeleteAsync(Uri requestUri, CancellationToken cancellationToken);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(string requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> PutAsync<TResponse>(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PutAsync(string requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PutAsync(Uri requestUri, object content);

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The data to post.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<JSendResponse> PutAsync(Uri requestUri, object content,
            CancellationToken cancellationToken);

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request);

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <typeparam name="TResponse">The type of the expected data.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
