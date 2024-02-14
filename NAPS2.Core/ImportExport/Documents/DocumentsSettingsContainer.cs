using System;
using System.Collections.Generic;
using System.Linq;
using NAPS2.Config;
namespace NAPS2.ImportExport.Documents
{
    public class DocumentsSettingsContainer
    {
        private readonly IUserConfigManager userConfigManager;

        private DocumentsSettings localDocumentsSettings;

        public DocumentsSettingsContainer(IUserConfigManager userConfigManager)
        {
            this.userConfigManager = userConfigManager;
        }

        public DocumentsSettings DocumentsSettings
        {
            get => localDocumentsSettings ?? userConfigManager.Config.DocumentsSettings ?? new DocumentsSettings();
            set => localDocumentsSettings = value;
        }
    }
}
