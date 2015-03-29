using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using System.Runtime.Serialization;
using JSend.Client.Properties;
using Newtonsoft.Json;

namespace JSend.Client
{
    /// <summary>
    /// Represents an exception that occurred while trying to parse a HTTP response message into a JSend response.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Type and Exception are required.")]
    public class JSendParseException : JSendResponseException
    {
        /// <summary>Initializes a new instance of <see cref="JSendParseException"/>.</summary>
        /// <param name="responseType">The type into which the parsing failed.</param>
        /// <param name="innerException">The exception that occurred while trying to parse a HTTP response message.</param>
        public JSendParseException(Type responseType, Exception innerException)
            : base(BuildMessage(responseType), innerException)
        {

        }

        private static string BuildMessage(Type responseType)
        {
            if (responseType == null)
                throw new ArgumentNullException("responseType");

            return string.Format(CultureInfo.InvariantCulture, StringResources.JSendParseException, responseType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSendResponseException"/> with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected JSendParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
