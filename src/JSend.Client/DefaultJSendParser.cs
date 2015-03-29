using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace JSend.Client
{
    /// <summary>
    /// Parses HTTP responses into JSend response objects.
    /// </summary>
    public class DefaultJSendParser : IJSendParser
    {
        private static readonly Lazy<Task<JsonSchema>> BaseSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.JSendResponseSchema.json"));

        private static readonly Lazy<Task<JsonSchema>> SuccessSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.SuccessResponseSchema.json"));

        private static async Task<JsonSchema> LoadSchema(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                var schema = await reader.ReadToEndAsync();
                return JsonSchema.Parse(schema);
            }
        }

        /// <summary>
        /// Parses the content of a <see cref="HttpResponseMessage"/> and returns a <see cref="JSendResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="httpResponseMessage">The HTTP response message to parse.</param>
        /// <returns>A task representings the parsed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JsonSchemaException">The HTTP response message body is not JSend formatted.</exception>
        /// <exception cref="JsonReaderException">The HTTP response body is not a JSON document.</exception>
        /// <exception cref="JsonSerializationException">The JSend data cannot be converted to an instance of type <typeparamref name="T"/>.</exception>
        public async Task<JSendResponse<T>> ParseAsync<T>(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null)
                throw new ArgumentNullException("httpResponseMessage");

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var json = JToken.Parse(content);
            json.Validate(await BaseSchema.Value);

            var status = json.Value<string>("status");
            switch (status)
            {
                case "success":
                    return await ParseSuccessMessageAsync<T>(json, httpResponseMessage);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parses the content of a <see cref="JToken"/> and returns a <see cref="JSendResponse{T}"/>
        /// representing a successful response.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="json">The <see cref="JToken"/> to parse.</param>
        /// <param name="responseMessage">The HTTP response message from where <paramref name="json"/> was extracted.</param>
        /// <returns>A task representing the successful <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JsonSchemaException"><paramref name="json"/> is not JSend formatted.</exception>
        /// <exception cref="JsonSerializationException">The JSend data cannot be converted to an instance of type <typeparamref name="T"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        public async Task<JSendResponse<T>> ParseSuccessMessageAsync<T>(JToken json, HttpResponseMessage responseMessage)
        {
            if (json == null) throw new ArgumentNullException("json");
            if (responseMessage == null) throw new ArgumentNullException("responseMessage");

            json.Validate(await SuccessSchema.Value);

            var dataToken = json["data"];
            if (dataToken.Type == JTokenType.Null)
                return new JSendResponse<T>(responseMessage);

            T data = dataToken.ToObject<T>();
            return new JSendResponse<T>(data, responseMessage);
        }
    }
}
