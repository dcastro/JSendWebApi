using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace JSend.Client
{
    /// <summary>
    /// Parses HTTP responses into JSend response objects.
    /// </summary>
    public interface IJSendParser
    {
        /// <summary>
        /// Parses the content of a <see cref="HttpResponseMessage"/> and returns a <see cref="JSendResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="httpResponseMessage">The HTTP response message to parse.</param>
        /// <returns>A task representings the parsed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JSendParseException">The HTTP response message could not be parsed.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        Task<JSendResponse<T>> ParseAsync<T>(HttpResponseMessage httpResponseMessage);
    }
}
