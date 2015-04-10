using System;
using System.Diagnostics.CodeAnalysis;

namespace JSend.Client
{
    /// <summary>
    /// Represents an exception that occurred while trying to parse a HTTP response message into a JSend response.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "A message must be provided.")]
    public class JSendParseException : JSendRequestException
    {
        /// <summary>Initializes a new instance of <see cref="JSendParseException"/>.</summary>
        /// <param name="message">The reason why the HTTP response message could not be parsed.</param>
        public JSendParseException(string message)
            : base(message)
        {
            
        }

        /// <summary>Initializes a new instance of <see cref="JSendParseException"/>.</summary>
        /// <param name="message">The reason why the HTTP response message could not be parsed.</param>
        /// <param name="innerException">The exception that occurred while trying to parse a HTTP response message.</param>
        public JSendParseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
