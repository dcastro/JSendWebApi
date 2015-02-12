using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class SuccessJSendResponse
    {
        [JsonProperty("status")]
        public string Status
        {
            get { return "success"; }
        }
    }
}
