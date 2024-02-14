using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;

namespace NAPS2.ImportExport.Squeeze
{
    public class SqueezeClient
    {
        private readonly SqueezeSettingsContainer squeezeSettingsContainer;
        private readonly HttpClient httpClient;
        private String soapSession = null;

        public SqueezeClient(SqueezeSettingsContainer squeezeSettingsContainer)
        {
            this.squeezeSettingsContainer = squeezeSettingsContainer;
            httpClient = new HttpClient();
        }
        
        #region REST

        /// <summary>
        /// Uploads a PDf to be processed. Uses the REST API
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="progressHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Rest_ProcessDocument(string filename, ProgressHandler progressHandler, CancellationToken cancellationToken)
        {
            var settings = squeezeSettingsContainer.SqueezeSettings;

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(settings.BatchClassId), "batchClassId");

            using (var stream = File.OpenRead(filename))
            {
                form.Add(new StreamContent(stream), "documentFile", filename);

                var authRaw = Encoding.ASCII.GetBytes(settings.Username + ":" + settings.Password);
                var authEnc = Convert.ToBase64String(authRaw);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authEnc);

                var rawResponse = await httpClient.PostAsync($"{settings.ServerUrl}/api/processDocument", form);

                if (!rawResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Squeeze upload failed: " + rawResponse.StatusCode + " " + rawResponse.ReasonPhrase);
                } else
                {
                    return true;
                }
            }
        }
        
        #endregion

        #region SOAP

        private async Task<bool> Soap_Login()
        {
            var settings = squeezeSettingsContainer.SqueezeSettings;

            var env = new XmlDocument();
            env.LoadXml(
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:doc=""http://xml.otris.de/ws/DOCUMENTS.xsd"">
   <soapenv:Header/>
   <soapenv:Body>
      <doc:login>
         <user>{settings.Username}</user>
         <principal>{settings.Client}</principal>
         <passwd>{settings.Password}</passwd>
         <locale>EN</locale>
      </doc:login>
   </soapenv:Body>
</soapenv:Envelope>");

            HttpWebRequest req;

            try
            {
                req = Soap_CreateWebRequest(settings, "login");
                Soap_InsertEnvelopeIntoWebRequest(env, req);
            }
            catch (Exception e)
            {
                throw new Exception("Creating request for login failed", e);
            }

            XmlDocument response;

            try
            {
                response = await Soap_PerformRequest(req);
            }
            catch (Exception e)
            {
                throw new Exception("Performing login request failed", e);
            }

            try
            {
                soapSession = response.GetElementsByTagName("session").Item(0)?.InnerText;
            }
            catch (Exception e)
            {
                throw new Exception("Getting session from response failed", e);
            }

            return true;
        }

        private async Task<bool> Soap_Logout()
        {
            if (soapSession == null)
            {
                return true;
            }
            
            var settings = squeezeSettingsContainer.SqueezeSettings;

            var env = new XmlDocument();
            env.LoadXml(
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:doc=""http://xml.otris.de/ws/DOCUMENTS.xsd"">
   <soapenv:Header>
      <doc:sessionID>{soapSession}</doc:sessionID>
   </soapenv:Header>
   <soapenv:Body>
      <doc:logout/>
   </soapenv:Body>
</soapenv:Envelope>");

            HttpWebRequest req;

            try
            {
                req = Soap_CreateWebRequest(settings, "logout");
                Soap_InsertEnvelopeIntoWebRequest(env, req);
            }
            catch (Exception e)
            {
                throw new Exception("Creating request for logout failed", e);
            }

            try
            {
                await Soap_PerformRequest(req);
            }
            catch (Exception e)
            {
                throw new Exception("Performing request for logout failed", e);
            }

            return true;
        }

        private bool Soap_IsLoggedIn()
        {
            return soapSession != null;
        }

        private async Task<bool> Soap_AssertLoggedIn()
        {
            if (!Soap_IsLoggedIn())
            {
                await Soap_Login();
            }

            return true;
        }

        /// <summary>
        /// Uploads a file to be processed. Uses the SOAP API
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="fields">Field name and field value</param>
        /// <param name="progressHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Soap_CreateFile(string filename, Dictionary<string, string> fields, ProgressHandler progressHandler, CancellationToken cancellationToken)
        {
            await Soap_AssertLoggedIn();

            var settings = squeezeSettingsContainer.SqueezeSettings;

            var bytes = File.ReadAllBytes(filename);
            var data = Convert.ToBase64String(bytes);
            var fieldsStr = "";
            
            if (fields != null && fields.Count > 0)
            {
                fieldsStr = string.Join("", fields
                    .Select(field =>
                    {
                        return $"<field><name>{field.Key}</name><value>{field.Value}</value></field>";
                    }));
            }

            var xml = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:doc=""http://xml.otris.de/ws/DOCUMENTS.xsd"">
    <soapenv:Header>
        <doc:sessionID>{soapSession}</doc:sessionID>
    </soapenv:Header>
    <soapenv:Body>
        <doc:createFile>
            <fileType></fileType>
            <fields>
                <field>
                    <name>batchclassid</name>
                    <value>{settings.BatchClassId}</value>
                </field>
                {fieldsStr}
            </fields>
            <addDocs>
                <document>
                    <name>SqueezeScannerUpload.pdf</name>
                    <register>Documents</register>
                    <data>{data}</data>
                </document>
            </addDocs>
        </doc:createFile>
    </soapenv:Body>
</soapenv:Envelope>";
            var env = new XmlDocument();
            try
            {
                env.LoadXml(xml);
            }
            catch (Exception e)
            {
                throw new Exception("Parsing XML for createFile request failed", e);
            }

            HttpWebRequest req;

            try
            {
                req = Soap_CreateWebRequest(settings, "createFile");
                Soap_InsertEnvelopeIntoWebRequest(env, req);
            }
            catch (Exception e)
            {
                throw new Exception("Creating createFile request failed", e);
            }

            try
            {
                await Soap_PerformRequest(req);
            }
            catch (Exception e)
            {
                throw new Exception("Performing createFile request failed", e);
            }
            
            await Soap_Logout();

            return true;
        }

        private static HttpWebRequest Soap_CreateWebRequest(SqueezeSettings settings, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(settings.ServerUrl + "/api/soapserver.php");
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void Soap_InsertEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static async Task<XmlDocument> Soap_PerformRequest(HttpWebRequest req)
        {
            using (WebResponse webResponse = await req.GetResponseAsync())
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    var soapResult = rd.ReadToEnd();
                    var response = new XmlDocument();
                    response.LoadXml(soapResult);
                    return response;
                }
            }
        }

        #endregion
    }
}
