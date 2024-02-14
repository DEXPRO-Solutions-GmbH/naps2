using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Protocol
{
    public class ProtocolMessage
    {
        [JsonProperty("cmd")]
        public string Command { get; set; }

        [JsonProperty("args")]
        public JToken Data { get; set; }

        public static ProtocolMessage FromProtocolData(string protocol)
        {
            var protocolMsg = protocol.Substring("d5scanner:".Length);
            ProtocolMessage msg = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(protocolMsg))
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(protocolMsg));
                    msg = JsonConvert.DeserializeObject<ProtocolMessage>(json);
                }
            }
            catch
            { }
            return msg;
        }
    }
}
