using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using JSend.Client.Properties;

namespace JSend.Client.Responses
{
    /// <summary>Represents a response received from a JSend API with "success" status, but no data.</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public class JSuccessResponse<T> : JSendResponse<T>
    {
        /// <summary>Initializes a new instance of <see cref="JSuccessResponse{T}"/>.</summary>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JSuccessResponse(HttpResponseMessage httpResponse) : base(httpResponse)
        {
        }

        /// <summary>
        /// Throws a <see cref="JSendRequestException"/> indicating that no data was received.
        /// </summary>
        /// <exception cref="JSendRequestException">No data was received.</exception>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "This is the same pattern used by Nullable<T>.")]
        public override T Data
        {
            get { throw new JSendRequestException(StringResources.SuccessResponseWithoutData); }
        }

        /// <summary>Returns false.</summary>
        public override bool HasData => false;
        
        /// <summary>Returns <see cref="JSendStatus.Success"/>.</summary>
        public override JSendStatus Status => JSendStatus.Success;

        /// <summary>Returns <see langword="null"/>.</summary>
        public override JSendError Error => null;
    }
}
