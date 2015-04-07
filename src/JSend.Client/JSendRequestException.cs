using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace JSend.Client
{
    /// <summary>
    /// Represents an exception that ocurred while querying a JSend API.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "A message must be provided.")]
    public class JSendRequestException : Exception
    {
        /// <summary>Initializes a new instance of <see cref="JSendRequestException"/>.</summary>
        /// <param name="message">A message that describes the current exception.</param>
        public JSendRequestException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of <see cref="JSendRequestException"/>.</summary>
        /// <param name="message">A message that describes the current exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public JSendRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
