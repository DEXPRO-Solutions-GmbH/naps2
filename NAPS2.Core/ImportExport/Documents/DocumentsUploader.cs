using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NAPS2.ImportExport.Documents.Models;
using System.IO;
using System.Net.Http.Headers;

namespace NAPS2.ImportExport.Documents
{
    public class DocumentsUploader
    {
        private readonly DocumentsSettingsContainer documentsSettingsContainer;
        private readonly HttpClient httpClient;

        public DocumentsUploader(DocumentsSettingsContainer documentsSettingsContainer)
        {
            this.documentsSettingsContainer = documentsSettingsContainer;
            httpClient = new HttpClient();
        }

        public async Task<bool> Upload(string filename, ProgressHandler progressHandler, CancellationToken cancellationToken)
        {
            var documentsSettings = documentsSettingsContainer.DocumentsSettings;
            var sessionId = documentsSettings.SessionId;
            var didLogin = false;
            if (sessionId == null)
            {
                sessionId = await Connect();
                didLogin = true;
            }
            await DoUpload(filename, sessionId, didLogin);
            if(didLogin)
                await Disconnect(sessionId);
            return true;
        }

        private async Task DoUpload(string filename, string sessionId, bool doCommit)
        {
            var documentsSettings = documentsSettingsContainer.DocumentsSettings;
            var message = new HttpRequestMessage(HttpMethod.Post, $"{documentsSettings.ServerUrl}/srv/uploadDrop{sessionId}");
            message.Headers.Add("fileName", Path.GetFileName(filename));
            if(doCommit)
                message.Headers.Add("fileEditCommit", "true");
            using (var stream = File.OpenRead(filename))
            {
                var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                message.Content = content;
                var rawResponse = await httpClient.SendAsync(message);
                if (rawResponse.IsSuccessStatusCode)
                {
                    var uploadResponse = await HttpHelpers.DeserializeContentAsync<UploadResponse>(rawResponse.Content);
                    if (!string.IsNullOrWhiteSpace(uploadResponse.ErrorMessage))
                        throw new Exception("Document upload failed: " + uploadResponse.ErrorMessage);
                    return;
                }
            }
            throw new Exception("Could not upload document");
        }

        private async Task<string> Connect()
        {
            var documentsSettings = documentsSettingsContainer.DocumentsSettings;
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "action", "loginUser" },
                { "login", documentsSettings.Username },
                { "password", documentsSettings.Password },
                { "pri", documentsSettings.Principal },
                { "appLogin", "true" }
            });
            var rawResponse = await httpClient.PostAsync($"{documentsSettings.ServerUrl}/srv/system", content);
            if (rawResponse.IsSuccessStatusCode)
            {
                var loginResponse = await HttpHelpers.DeserializeContentAsync<LoginResponse>(rawResponse.Content);
                if (loginResponse.IsLoggedIn)
                {
                    return loginResponse.SessionId;
                }
            }
            throw new Exception("Could not log in to DOCUMENTS");
        }

        private async Task Disconnect(string sessionId)
        {
            var documentsSettings = documentsSettingsContainer.DocumentsSettings;
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "action", "logoutUser" },
                { "pri", documentsSettings.Principal }
            });
            var rawResponse = await httpClient.PostAsync($"{documentsSettings.ServerUrl}/srv/system{sessionId}", content);
            if (rawResponse.IsSuccessStatusCode)
            {
                var loginResponse = await HttpHelpers.DeserializeContentAsync<LoginResponse>(rawResponse.Content);
                if (!loginResponse.IsLoggedIn)
                    return;
            }
            throw new Exception("Could not log out of DOCUMENTS");
        }
    }
}
