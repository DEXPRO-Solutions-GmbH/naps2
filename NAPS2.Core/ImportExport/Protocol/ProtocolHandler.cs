using NAPS2.ImportExport.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Protocol
{
    public class ProtocolHandler
    {
        private readonly DocumentsSettingsContainer documentsSettingsContainer;

        public ProtocolHandler(DocumentsSettingsContainer documentsSettingsContainer)
        {
            this.documentsSettingsContainer = documentsSettingsContainer;
        }

        public void HandleProtocol(string protocol)
        {
            try
            {
                var msg = ProtocolMessage.FromProtocolData(protocol);
                if (msg != null)
                {
                    switch (msg.Command)
                    {
                        case "documentsConfig":
                            documentsSettingsContainer.DocumentsSettings = msg.Data.ToObject<DocumentsSettings>();
                            break;
                    }
                }
            }
            catch { };
        }
    }
}
