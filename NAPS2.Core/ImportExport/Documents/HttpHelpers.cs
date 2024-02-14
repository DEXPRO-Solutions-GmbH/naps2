using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Documents
{
    public static class HttpHelpers
    {
        public async static Task<T> DeserializeContentAsync<T>(HttpContent response) where T : new()
        {
            var jsonSerializer = new JsonSerializer();
            using (var stream = await response.ReadAsStreamAsync())
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
