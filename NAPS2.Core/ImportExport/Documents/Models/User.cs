using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Documents.Models
{
    public class User
    {
        [JsonProperty("firstName")]
        public string FirstName { get; private set; }

        [JsonProperty("lastName")]
        public string LastName { get; private set; }

        [JsonProperty("email")]
        public string EMail { get; private set; }
    }
}
