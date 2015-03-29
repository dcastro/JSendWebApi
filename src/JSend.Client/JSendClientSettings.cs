using System.Text;
using Newtonsoft.Json;

namespace JSend.Client
{
    /// <summary>
    /// Specifies the settings of a <see cref="JSendClient"/>.
    /// </summary>
    public class JSendClientSettings
    {
        /// <summary>Gets or sets the parser used to process JSend-formatted responses.</summary>
        public IJSendParser Parser { get; set; }

        /// <summary>Gets or sets the encoding used to format a request's content.</summary>
        public Encoding Encoding { get; set; }

        /// <summary>Gets or sets the settings used to serialize the content of a request.</summary>
        public JsonSerializerSettings SerializerSettings { get; set; }
    }
}
