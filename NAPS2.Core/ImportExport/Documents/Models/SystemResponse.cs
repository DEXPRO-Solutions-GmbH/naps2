using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Documents.Models
{
    public abstract class SystemResponse
    {
        [JsonProperty("userState")]
        public bool IsLoggedIn { get; set; }

        [JsonProperty("requestData")]
        public dynamic RequestData { get; set; }

        [JsonProperty("sessionString")]
        public string SessionId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
