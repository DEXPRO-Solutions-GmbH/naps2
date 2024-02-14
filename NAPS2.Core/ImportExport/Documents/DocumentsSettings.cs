using System;
using System.Collections.Generic;
using System.Linq;

namespace NAPS2.ImportExport.Documents
{
    public class DocumentsSettings
    {
        public DocumentsSettings()
        {
            ServerUrl = "http://localhost:8080/documents5";
            Principal = "";
            Username = "";
            Password = "";
            SessionId = null;
        }

        public string ServerUrl { get; set; }

        public string Principal { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SessionId { get; set; }

        public bool IsMissingDetails
        {
            get => string.IsNullOrWhiteSpace(ServerUrl)
                || string.IsNullOrWhiteSpace(Principal)
                || (
                    string.IsNullOrWhiteSpace(SessionId)
                    && (
                        string.IsNullOrWhiteSpace(Username)
                        || string.IsNullOrWhiteSpace(Password)
                    )
                );
        }
    }
}
