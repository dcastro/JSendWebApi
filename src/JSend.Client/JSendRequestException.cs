using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace JSend.Client
{
    /// <summary>
    /// Represents an exception that ocurred while querying a JSend API.
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="JSendRequestException"/> with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected JSendRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
