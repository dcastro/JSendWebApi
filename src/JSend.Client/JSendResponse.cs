using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http;
using System.Text;
using JSend.Client.Properties;

namespace JSend.Client
{
    /// <summary>Represents the response received from a JSend API.</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public class JSendResponse<T> : IDisposable
    {
        private readonly T _data;
        private readonly bool _hasData;
        private readonly JSendError _error;
        private readonly HttpResponseMessage _httpResponse;
        private readonly JSendStatus _status;

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a successful response.
        /// </summary>
        /// <param name="data">The data returned by the API.</param>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JSendResponse(T data, HttpResponseMessage httpResponse)
            : this(data, true, null, httpResponse)
        {
            _status = JSendStatus.Success;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a successful response.
        /// </summary>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JSendResponse(HttpResponseMessage httpResponse)
            : this(default(T), false, null, httpResponse)
        {
            _status = JSendStatus.Success;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a failure/error response.
        /// </summary>
        /// <param name="error">The error details.</param>
        /// <param name="httpResponse">The HTTP response message.</param>
        public JSendResponse(JSendError error, HttpResponseMessage httpResponse)
            : this(default(T), false, error, httpResponse)
        {
            if (error == null) throw new ArgumentNullException("error");

            Contract.Assert(error.Status != JSendStatus.Success);
            _status = error.Status;
        }

        private JSendResponse(T data, bool hasData, JSendError error, HttpResponseMessage httpResponse)
        {
            if (httpResponse == null) throw new ArgumentNullException("httpResponse");

            _data = data;
            _hasData = hasData;
            _error = error;
            _httpResponse = httpResponse;
        }

        /// <summary>
        /// Gets the data returned by the API.
        /// If none was returned or if the response was not successful,
        /// a <see cref="JSendRequestException"/> will be thrown.
        /// </summary>
        /// <exception cref="JSendRequestException">The request was not successful or did not return any data.</exception>        
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "This is the same pattern used by Nullable<T>.")]
        public T Data
        {
            get
            {
                EnsureSuccessStatus();

                if (!HasData)
                    throw new JSendRequestException(StringResources.SuccessResponseWithoutData);
                return _data;
            }
        }

        /// <summary>
        /// Gets whether the response contains data.
        /// </summary>
        public bool HasData
        {
            get { return _hasData; }
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
        public HttpResponseMessage HttpResponse
        {
            get { return _httpResponse; }
        }

        /// <summary>
        /// Gets the error object with the details of why the request failed.
        /// <see langword="null"/> if the request was successful.
        /// </summary>
        public JSendError Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Returns the response's data, if any;
        /// otherwise the default value of <typeparamref name="T"/> is returned.
        /// </summary>
        /// <returns>The response's data or the default vale of <typeparamref name="T"/>.</returns>
        [Pure]
        public T GetDataOrDefault()
        {
            return GetDataOrDefault(default(T));
        }

        /// <summary>
        /// Returns the response's data, if any;
        /// otherwise <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <param name="defaultValue">The value to return if the response does not contain any data.</param>
        /// <returns>The response's data or <paramref name="defaultValue"/>.</returns>
        [Pure]
        public T GetDataOrDefault(T defaultValue)
        {
            return HasData ? Data : defaultValue;
        }

        /// <summary>Throws an exception if <see cref="IsSuccess"/> is <see langword="false"/>.</summary>
        /// <returns>Returns itself if the call is successful.</returns>
        /// <exception cref="JSendRequestException">The request was not successful.</exception>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We explicitly want the lower-case string here")]
        public JSendResponse<T> EnsureSuccessStatus()
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
        private void Dispose(bool disposing)
        {
            if (disposing)
                HttpResponse.Dispose();
        }

        private bool Equals(JSendResponse<T> other)
        {
            return _status == other.Status &&
                   object.Equals(_error, other._error) &&
                   object.Equals(_httpResponse, other._httpResponse) &&
                   EqualityComparer<T>.Default.Equals(_data, other._data) &&
                   _hasData.Equals(other._hasData);
        }

        /// <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="JSendResponse{T}"/>.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((JSendResponse<T>) obj);
        }

        /// <summary>Returns whether the two operands are equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are equal; otherwise, <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification =
            "Non-sealed classes should not implemented IEquatable<T>. See http://blog.mischel.com/2013/01/05/inheritance-and-iequatable-do-not-mix/" +
            "and http://stackoverflow.com/q/1868316/857807")]
        public static bool operator ==(JSendResponse<T> left, JSendResponse<T> right)
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
        public static bool operator !=(JSendResponse<T> left, JSendResponse<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary>Serves as a hash function for this <see cref="JSendResponse{T}"/>.</summary>
        /// <returns>A hash code for this <see cref="JSendResponse{T}"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _status;
                hashCode = (hashCode*397) ^ (_error != null ? _error.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _httpResponse.GetHashCode();
                hashCode = (hashCode*397) ^ EqualityComparer<T>.Default.GetHashCode(_data);
                hashCode = (hashCode*397) ^ _hasData.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>Returns a string that represents the <see cref="JSendResponse{T}"/>.</summary>
        /// <returns>A string that represents the <see cref="JSendResponse{T}"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (HasData)
                sb.AppendFormat(CultureInfo.InvariantCulture, "Data: {0}, ", Data);

            sb.AppendFormat(CultureInfo.InvariantCulture, "Status: {0}", Status);

            if (Error != null)
                sb.AppendFormat(CultureInfo.CurrentCulture, ", Error: {{{0}}}", Error);

            sb.AppendFormat(CultureInfo.CurrentCulture, ", HttpResponse: {{{0}}}", HttpResponse);

            return sb.ToString();
        }
    }
}
