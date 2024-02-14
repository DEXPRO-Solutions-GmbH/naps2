using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Documents.Models
{
    public class LoginResponse : SystemResponse
    {
        [JsonProperty("responseOutput")]
        public User User { get; set; }
    }
}
