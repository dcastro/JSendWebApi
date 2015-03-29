using System;
using System.Diagnostics.CodeAnalysis;
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
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<T>> GetAsync<T>(string requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<T>> GetAsync<T>(Uri requestUri);

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<T>> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken);
    }
}
