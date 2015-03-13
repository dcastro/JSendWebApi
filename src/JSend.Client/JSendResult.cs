using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http;
using JSend.Client.Properties;

namespace JSend.Client
{
    /// <summary>Represents the result of a request to a JSend API.</summary>
    public class JSendResult : IDisposable
    {
        private readonly JSendStatus _status;
        private readonly JSendError _error;
        private readonly HttpResponseMessage _responseMessage;

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResult"/> representing a successful response.
        /// </summary>
        /// <param name="responseMessage">The HTTP response message.</param>
        public JSendResult(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null) throw new ArgumentNullException("responseMessage");

            _status = JSendStatus.Success;
            _responseMessage = responseMessage;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResult"/> representing a failure/error response.
        /// </summary>
        /// <param name="error">The error details.</param>
        /// <param name="responseMessage">The HTTP response message.</param>
        public JSendResult(JSendError error, HttpResponseMessage responseMessage)
        {
            if (error == null) throw new ArgumentNullException("error");
            if (responseMessage == null) throw new ArgumentNullException("responseMessage");

            Contract.Assert(error.Status != JSendStatus.Success);

            _status = error.Status;
            _error = error;
            _responseMessage = responseMessage;
        }

        /// <summary>Indicates whether the JSend response had a status of "success".</summary>
        public bool IsSuccess
        {
            get { return Status == JSendStatus.Success; }
        }

        /// <summary>Gets the status of the JSend response.</summary>
        public JSendStatus Status
        {
            get { return _status; }
        }

        /// <summary>Gets the HTTP response message.</summary>
        public HttpResponseMessage ResponseMessage
        {
            get { return _responseMessage; }
        }

        /// <summary>
        /// Gets the error object with the details of why the request failed.
        /// <see langword="null"/> if the request was successful.
        /// </summary>
        public JSendError Error
        {
            get { return _error; }
        }

        /// <summary>Throws an exception if <see cref="IsSuccess"/> is <see langword="false"/>.</summary>
        /// <returns>Returns itself if the call is successful.</returns>
        /// <exception cref="JSendRequestException">The request was not successful.</exception>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "We explicitly want the lower-case string here")]
        public JSendResult EnsureSuccessStatus()
        {
            if (!IsSuccess)
            {
                // If the status is not "success", an exception is thrown - the behavior is similar
                // to a failed request (i.e., connection failure). Users don't expect to dispose the content
                // in this case. If an exception is thrown, the object is responsible for
                // cleaning up its state.
                Dispose();

                throw new JSendRequestException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        StringResources.UnsuccessfulResponse, Status.ToString().ToLowerInvariant()));
            }

            return this;
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
                ResponseMessage.Dispose();
        }
    }
}
