using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Documents.Models
{
    public class UploadResponse
    {
        [JsonProperty("fileId")]
        public string FileId { get; set; }

        [JsonProperty("registerId")]
        public string RegisterId { get; set; }

        [JsonProperty("attachmentId")]
        public string DocumentId { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
