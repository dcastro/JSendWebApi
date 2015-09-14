using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http;
using JSend.Client.Properties;

namespace JSend.Client.Responses
{
    /// <summary>Represents a response received from a JSend API with status equal to "fail" or "error".</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public class JErrorResponse<T> : JSendResponse<T>
    {
        /// <summary>Initializes a new instance of <see cref="JErrorResponse{T}"/>.</summary>
        /// <param name="error">The error details.</param>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JErrorResponse(JSendError error, HttpResponseMessage httpResponse) : base(httpResponse)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            Contract.Assert(error.Status != JSendStatus.Success);
            Error = error;
        }

        /// <summary>
        /// Throws a <see cref="JSendRequestException"/> indicating that the request was
        /// not successful and, therefore, no data was received.
        /// </summary>
        /// <exception cref="JSendRequestException">The request was not successful.</exception>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "This is the same pattern used by Nullable<T>.")]
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We explicitly want the lower-case string here")]
        public override T Data
        {
            get
            {
                throw new JSendRequestException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        StringResources.UnsuccessfulResponse, Status.ToString().ToLowerInvariant()));
            }
        }

        /// <summary>Returns false.</summary>
        public override bool HasData => false;

        /// <summary>Gets the status of the JSend response.</summary>
        public override JSendStatus Status => Error.Status;

        /// <summary>Gets the error object with the details of why the request failed.</summary>
        public override JSendError Error { get; }
    }
}
