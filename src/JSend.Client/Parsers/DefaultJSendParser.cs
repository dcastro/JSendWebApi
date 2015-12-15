using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using JSend.Client.Extensions;
using JSend.Client.Properties;
using JSend.Client.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using static JSend.Client.JSendSchemas;

namespace JSend.Client.Parsers
{
    /// <summary>
    /// Parses HTTP responses into JSend response objects.
    /// </summary>
    public class DefaultJSendParser : IJSendParser
    {
        /// <summary>Gets an instance of <see cref="DefaultJSendParser"/>.</summary>
        public static DefaultJSendParser Instance { get; } = new DefaultJSendParser();

        /// <summary>
        /// Parses the content of a <see cref="HttpResponseMessage"/> and returns a <see cref="JSendResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="serializerSettings">The settings used to deserialize the response.</param>
        /// <param name="httpResponse">The HTTP response message to parse.</param>
        /// <returns>A task representings the parsed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JSendParseException">The HTTP response message could not be parsed.</exception>
        [Pure]
        public async Task<JSendResponse<T>> ParseAsync<T>(JsonSerializerSettings serializerSettings,
            HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            if (httpResponse.Content == null)
                throw new JSendParseException(StringResources.ResponseWithoutContent);

            var content = await httpResponse.Content.ReadAsStringAsync().IgnoreContext();

            try
            {
                var json = JsonConvert.DeserializeObject<JToken>(content, serializerSettings);
                json.Validate(await GetBaseSchemaAsync().IgnoreContext());

                var status = json.Value<string>("status");
                switch (status)
                {
                    case "success":
                        return await ParseSuccessMessageAsync<T>(json, serializerSettings, httpResponse).IgnoreContext();
                    case "fail":
                        return await ParseFailMessageAsync<T>(json, httpResponse).IgnoreContext();
                    case "error":
                        return await ParseErrorMessageAsync<T>(json, httpResponse).IgnoreContext();
                    default:
                        Contract.Assert(false);
                        return null;
                }
            }
            catch (JsonException ex)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    StringResources.ResponseParseError,
                    typeof (JSendResponse<T>), Environment.NewLine, content);

                throw new JSendParseException(message, ex);
            }
        }

        /// <summary>
        /// Parses the content of a <see cref="JToken"/> and returns a <see cref="JSendResponse{T}"/>
        /// representing a successful response.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="json">The <see cref="JToken"/> to parse.</param>
        /// <param name="serializerSettings">The settings used to deserialize the response.</param>
        /// <param name="responseMessage">The HTTP response message from where <paramref name="json"/> was extracted.</param>
        /// <returns>A task representing the successful <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JsonSchemaException"><paramref name="json"/> is not JSend formatted.</exception>
        /// <exception cref="JsonSerializationException">The JSend data cannot be converted to an instance of type <typeparamref name="T"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        [Pure]
        public static async Task<JSendResponse<T>> ParseSuccessMessageAsync<T>(JToken json,
            JsonSerializerSettings serializerSettings, HttpResponseMessage responseMessage)
        {
            json.Validate(await GetSuccessSchemaAsync().IgnoreContext());

            var dataToken = json["data"];
            if (dataToken.Type == JTokenType.Null)
                return new JSuccessResponse<T>(responseMessage);

            var serializer = JsonSerializer.Create(serializerSettings);

            T data = dataToken.ToObject<T>(serializer);
            return new JSuccessWithDataResponse<T>(data, responseMessage);
        }

        /// <summary>
        /// Parses the content of a <see cref="JToken"/> and returns a <see cref="JSendResponse{T}"/>
        /// representing a fail response.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="json">The <see cref="JToken"/> to parse.</param>
        /// <param name="responseMessage">The HTTP response message from where <paramref name="json"/> was extracted.</param>
        /// <returns>A task representing the failed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JsonSchemaException"><paramref name="json"/> is not JSend formatted.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        [Pure]
        public static async Task<JSendResponse<T>> ParseFailMessageAsync<T>(JToken json, HttpResponseMessage responseMessage)
        {
            json.Validate(await GetFailSchemaAsync().IgnoreContext());

            var dataToken = json["data"];
            var error = new JSendError(JSendStatus.Fail, null, null, dataToken);
            return new JErrorResponse<T>(error, responseMessage);
        }

        /// <summary>
        /// Parses the content of a <see cref="JToken"/> and returns a <see cref="JSendResponse{T}"/>
        /// representing an error response.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="json">The <see cref="JToken"/> to parse.</param>
        /// <param name="responseMessage">The HTTP response message from where <paramref name="json"/> was extracted.</param>
        /// <returns>A task representing the error <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JsonSchemaException"><paramref name="json"/> is not JSend formatted.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need to return a response asynchronously.")]
        [Pure]
        public static async Task<JSendResponse<T>> ParseErrorMessageAsync<T>(JToken json, HttpResponseMessage responseMessage)
        {
            json.Validate(await GetErrorSchemaAsync().IgnoreContext());

            var message = json.Value<string>("message");
            var code = json.Value<int?>("code");
            var dataToken = json["data"];

            // check if the "data" key exists, but has a null value
            if (dataToken != null && dataToken.Type == JTokenType.Null)
                dataToken = null;

            var error = new JSendError(JSendStatus.Error, message, code, dataToken);
            return new JErrorResponse<T>(error, responseMessage);
        }
    }
}
