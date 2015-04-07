using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http;
using System.Text;
using JSend.Client.Properties;

namespace JSend.Client
{
    /// <summary>Represents the response received from a JSend API.</summary>
    public class JSendResponse : IDisposable
    {
        private readonly JSendStatus _status;
        private readonly JSendError _error;
        private readonly HttpResponseMessage _httpResponseMessage;

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse"/> representing a successful response.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        public JSendResponse(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null) throw new ArgumentNullException("httpResponseMessage");

            _status = JSendStatus.Success;
            _httpResponseMessage = httpResponseMessage;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse"/> representing a failure/error response.
        /// </summary>
        /// <param name="error">The error details.</param>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        public JSendResponse(JSendError error, HttpResponseMessage httpResponseMessage)
        {
            if (error == null) throw new ArgumentNullException("error");
            if (httpResponseMessage == null) throw new ArgumentNullException("httpResponseMessage");

            Contract.Assert(error.Status != JSendStatus.Success);

            _status = error.Status;
            _error = error;
            _httpResponseMessage = httpResponseMessage;
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
        public HttpResponseMessage HttpResponseMessage
        {
            get { return _httpResponseMessage; }
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
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We explicitly want the lower-case string here")]
        public JSendResponse EnsureSuccessStatus()
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
                HttpResponseMessage.Dispose();
        }

        private bool Equals(JSendResponse other)
        {
            return _status == other._status &&
                   Equals(_error, other._error) &&
                   _httpResponseMessage.Equals(other._httpResponseMessage);
        }

        /// <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="JSendResponse"/>.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((JSendResponse) obj);
        }

        /// <summary>Serves as a hash function for this <see cref="JSendResponse"/>.</summary>
        /// <returns>A hash code for this <see cref="JSendResponse"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _status;
                hashCode = (hashCode*397) ^ (_error != null ? _error.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _httpResponseMessage.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>Returns whether the two operands are equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are equal; otherwise, <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification =
            "Non-sealed classes should not implemented IEquatable<T>. See http://blog.mischel.com/2013/01/05/inheritance-and-iequatable-do-not-mix/" +
            "and http://stackoverflow.com/q/1868316/857807")]
        public static bool operator ==(JSendResponse left, JSendResponse right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns whether the two operands are not equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are not equal; otherwise, <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification =
            "Non-sealed classes should not implemented IEquatable<T>. See http://blog.mischel.com/2013/01/05/inheritance-and-iequatable-do-not-mix/" +
            "and http://stackoverflow.com/q/1868316/857807")]
        public static bool operator !=(JSendResponse left, JSendResponse right)
        {
            return !Equals(left, right);
        }

        /// <summary>Returns a string that represents the <see cref="JSendResponse"/>.</summary>
        /// <returns>A string that represents the <see cref="JSendResponse"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "Status: {0}", Status);

            if (Error != null)
                sb.AppendFormat(CultureInfo.CurrentCulture, ", Error: {{{0}}}", Error);

            sb.AppendFormat(CultureInfo.CurrentCulture, ", HttpResponseMessage: {{{0}}}", HttpResponseMessage);

            return sb.ToString();
        }
    }
}
