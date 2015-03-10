using System;
using Newtonsoft.Json;

namespace JSend.Client
{
    /// <summary>A JSend fail response.</summary>
    public class FailResponse : IJSendResponse
    {
        private readonly object _data;

        /// <summary>Initializes a new instance of <see cref="FailResponse"/>.</summary>
        /// <param name="data">A wrapper for the details of why the request failed.</param>
        public FailResponse(object data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        /// <summary>Gets the status of this response, always set to <see cref="JSendStatus.Fail"/>.</summary>
        [JsonProperty("status")]
        public JSendStatus Status
        {
            get { return JSendStatus.Fail; }
        }

        /// <summary>Gets the wrapper for the details of why the request failed.</summary>
        [JsonProperty("data")]
        public object Data
        {
            get { return _data; }
        }
        
        /// <summary>Returns a string that represents the current response.</summary>
        /// <returns>A string that represents the current response.</returns>
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
