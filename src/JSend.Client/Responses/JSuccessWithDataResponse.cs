using System.Net.Http;

namespace JSend.Client.Responses
{
    /// <summary>Represents a response received from a JSend API with "success" status and data.</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public class JSuccessWithDataResponse<T> : JSendResponse<T>
    {
        /// <summary>Initializes a new instance of <see cref="JSuccessWithDataResponse{T}"/>.</summary>
        /// <param name="data">The data returned by the API.</param>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JSuccessWithDataResponse(T data, HttpResponseMessage httpResponse) : base(httpResponse)
        {
            Data = data;
        }

        /// <summary>Gets the data returned by the API.</summary>
        public override T Data { get; }

        /// <summary>Returns true.</summary>
        public override bool HasData => true;

        /// <summary>Returns <see cref="JSendStatus.Success"/>.</summary>
        public override JSendStatus Status => JSendStatus.Success;

        /// <summary>Returns <see langword="null"/>.</summary>
        public override JSendError Error => null;
    }
}
