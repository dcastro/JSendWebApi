using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using JSend.Client.Properties;

namespace JSend.Client
{
    /// <summary>Represents the response received from a JSend API.</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public sealed class JSendResponse<T> : JSendResponse, IEquatable<JSendResponse<T>>
    {
        private readonly T _data;
        private readonly bool _hasData;

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a successful response.
        /// </summary>
        /// <param name="data">The data returned by the API.</param>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        public JSendResponse(T data, HttpResponseMessage httpResponseMessage)
            : base(httpResponseMessage)
        {
            _data = data;
            _hasData = true;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a successful response.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        public JSendResponse(HttpResponseMessage httpResponseMessage)
            : base(httpResponseMessage)
        {
            _hasData = false;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JSendResponse{T}"/> representing a failure/error response.
        /// </summary>
        /// <param name="error">The error details.</param>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        public JSendResponse(JSendError error, HttpResponseMessage httpResponseMessage)
            : base(error, httpResponseMessage)
        {
            _hasData = false;
        }

        /// <summary>
        /// Gets the data returned by the API.
        /// If none was returned or if the response was not successful,
        /// a <see cref="JSendResponseException"/> will be thrown.
        /// </summary>
        /// <exception cref="JSendResponseException">The request was not successful or did not return any data.</exception>        
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "This is the same pattern used by Nullable<T>.")]
        public T Data
        {
            get
            {
                EnsureSuccessStatus();

                if (!HasData)
                    throw new JSendResponseException(StringResources.SuccessResponseWithoutData);
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

        /// <summary>
        /// Returns the response's data, if any;
        /// otherwise the default value of <typeparamref name="T"/> is returned.
        /// </summary>
        /// <returns>The response's data or the default vale of <typeparamref name="T"/>.</returns>
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
        public T GetDataOrDefault(T defaultValue)
        {
            return HasData ? Data : defaultValue;
        }

        /// <summary>Throws an exception if <see cref="JSendResponse.IsSuccess"/> is <see langword="false"/>.</summary>
        /// <returns>Returns itself if the call is successful.</returns>
        /// <exception cref="JSendResponseException">The request was not successful.</exception>
        public new JSendResponse<T> EnsureSuccessStatus()
        {
            base.EnsureSuccessStatus();

            return this;
        }

        /// <summary>Determines whether the specified <see cref="JSendResponse{T}"/> is equal to the current <see cref="JSendResponse{T}"/>.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified error is equal to the current error; otherwise, <see langword="false"/>.</returns>
        public bool Equals(JSendResponse<T> other)
        {
            return base.Equals(other) &&
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

            var other = obj as JSendResponse<T>;
            if (other == null) return false;

            return Equals(other);
        }

        /// <summary>Serves as a hash function for this <see cref="JSendResponse{T}"/>.</summary>
        /// <returns>A hash code for this <see cref="JSendResponse"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ EqualityComparer<T>.Default.GetHashCode(_data);
                hashCode = (hashCode*397) ^ _hasData.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>Returns whether the two operands are equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(JSendResponse<T> left, JSendResponse<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns whether the two operands are not equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(JSendResponse<T> left, JSendResponse<T> right)
        {
            return !Equals(left, right);
        }
    }
}
