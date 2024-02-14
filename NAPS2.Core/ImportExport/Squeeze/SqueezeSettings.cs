using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAPS2.ImportExport.Squeeze
{
    public class SqueezeSettings
    {
        public SqueezeSettings()
        {
            ServerUrl = "http://squeeze.local.network";
            Client = "squeeze.local.network";
            Username = "";
            Password = "";
            BatchClassId = "1";
            BarcodeFieldName = "Barcode";
        }

        public string ServerUrl { get; set; }

        public string Client { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string BatchClassId { get; set; }
        
        public string BarcodeFieldName { get; set; }

        public bool IsMissingDetails
        {
            get => string.IsNullOrWhiteSpace(ServerUrl)
                   || string.IsNullOrWhiteSpace(Username)
                   || string.IsNullOrWhiteSpace(Password)
                   || string.IsNullOrWhiteSpace(BatchClassId);
        }
    }
}
