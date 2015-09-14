using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using JSend.Client.Properties;
using Newtonsoft.Json.Linq;

namespace JSend.Client
{
    /// <summary>Represents an error returned by a JSend API.</summary>
    public sealed class JSendError : IEquatable<JSendError>
    {
        /// <summary>Initializes a new instance of <see cref="JSendError"/>.</summary>
        /// <param name="status">
        /// The status of the response. Must be either <see cref="JSendStatus.Error"/> or <see cref="JSendStatus.Fail"/>.
        /// </param>
        /// <param name="message">
        /// A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.
        /// </param>
        /// <param name="code">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data">A generic wrapper for the details of why the request failed.</param>
        public JSendError(JSendStatus status, string message, int? code, JToken data)
        {
            if (status == JSendStatus.Success)
                throw new ArgumentException(StringResources.ErrorWithSuccessStatus, nameof(status));

            Status = status;
            Message = message;
            Code = code;
            Data = data;
        }

        /// <summary>Gets the status of this response.</summary>
        public JSendStatus Status { get; }

        /// <summary>Gets the error message explaining what went wrong.</summary>
        public string Message { get; }

        /// <summary>Gets the numeric error code corresponding to the error.</summary>
        public int? Code { get; }

        /// <summary>Gets the generic data container.</summary>
        public JToken Data { get; }

        /// <summary>Determines whether the specified <see cref="JSendError"/> is equal to the current <see cref="JSendError"/>.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified error is equal to the current error; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public bool Equals(JSendError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Status == other.Status &&
                   Code == other.Code &&
                   string.Equals(Message, other.Message, StringComparison.Ordinal) &&
                   JToken.EqualityComparer.Equals(Data, other.Data);
        }

        /// <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="JSendError"/>.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => Equals(obj as JSendError);

        /// <summary>Serves as a hash function for this <see cref="JSendError"/>.</summary>
        /// <returns>A hash code for this <see cref="JSendError"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Status;
                hashCode = (hashCode*397) ^ (Message != null ? StringComparer.Ordinal.GetHashCode(Message) : 0);
                hashCode = (hashCode*397) ^ Code.GetHashCode();
                hashCode = (hashCode*397) ^ JToken.EqualityComparer.GetHashCode(Data);
                return hashCode;
            }
        }

        /// <summary>Returns whether the two operands are equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(JSendError left, JSendError right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns whether the two operands are not equal.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both operands are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(JSendError left, JSendError right)
        {
            return !Equals(left, right);
        }

        /// <summary>Returns a string that represents the <see cref="JSendError"/>.</summary>
        /// <returns>A string that represents the <see cref="JSendError"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "Status: {0}", Status);

            if (Message != null)
                sb.AppendFormat(CultureInfo.CurrentCulture, ", Message: {0}", Message);

            if (Code.HasValue)
                sb.AppendFormat(CultureInfo.CurrentCulture, ", Code: {0}", Code.Value);

            if (Data != null)
                sb.AppendFormat(CultureInfo.CurrentCulture, ", Data: {0}",
                    Data.Type == JTokenType.Null ? "<null>" : Data);

            return sb.ToString();
        }
    }
}
