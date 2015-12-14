using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using JSend.Client.Extensions;
using Newtonsoft.Json.Schema;

namespace JSend.Client
{
    /// <summary>
    /// Loads schemas for validating JSend responses.
    /// </summary>
    public static class JSendSchemas
    {
        private static readonly Lazy<Task<JsonSchema>> BaseSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.JSendResponseSchema.json"));

        private static readonly Lazy<Task<JsonSchema>> SuccessSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.SuccessResponseSchema.json"));

        private static readonly Lazy<Task<JsonSchema>> FailSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.FailResponseSchema.json"));

        private static readonly Lazy<Task<JsonSchema>> ErrorSchema = new Lazy<Task<JsonSchema>>(
            () => LoadSchema("JSend.Client.Schemas.ErrorResponseSchema.json"));

        /// <summary>
        /// Loads a schema for validating a basic JSend response,
        /// which must at least contain a status key with a valid value.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performs async I/O.")]
        public static Task<JsonSchema> GetBaseSchemaAsync() => BaseSchema.Value;

        /// <summary>Loads a schema for validating a JSend response with success status.</summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performs async I/O.")]
        public static Task<JsonSchema> GetSuccessSchemaAsync() => SuccessSchema.Value;

        /// <summary>Loads a schema for validating a JSend response with fail status.</summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performs async I/O.")]
        public static Task<JsonSchema> GetFailSchemaAsync() => FailSchema.Value;

        /// <summary>Loads a schema for validating a JSend response with error status.</summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performs async I/O.")]
        public static Task<JsonSchema> GetErrorSchemaAsync() => ErrorSchema.Value;

        private static async Task<JsonSchema> LoadSchema(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                var schema = await reader.ReadToEndAsync().IgnoreContext();
                return JsonSchema.Parse(schema);
            }
        }
    }
}
