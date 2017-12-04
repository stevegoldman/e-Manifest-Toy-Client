using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;


namespace eManifestServicesClient
{
    public class eManifestServices
    {
        private string _apiID;
        private string _apiKey;
        private string _baseURL;
        // According to Microsoft, HttpClient is intended to be created once and used thoughout
        // the life of the application.
        private HttpClient _client;
       

        
        public eManifestServices()
        {
            // These values are in the app.config file for the main assembly that's using this class library.
            // apiID and apiKey are specific to a Site Manager user in EPA's RCRAInfo app.
            // Use Visual Studios "Settings" functionality in Production.  ConfigurationManager is deprecated (but simpler).           
            _apiID = ConfigurationManager.AppSettings["apiID"];
            _apiKey = ConfigurationManager.AppSettings["apiKey"];
            _baseURL = ConfigurationManager.AppSettings["baseURL"];
            // It seems that the service provides a session cookie, which causes a repeated post to fail.
            // Cookies are disabled.
            WebRequestHandler handler = new WebRequestHandler();
            handler.UseCookies = false;
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(_baseURL);
        }
        public async Task<string> authenticate()
        {
            HttpResponseMessage resp = await _client.GetAsync(String.Format("auth/{0}/{1}", _apiID, _apiKey));
            string jsonAuthResponse = await resp.Content.ReadAsStringAsync();
            String authToken = (JsonConvert.DeserializeObject<AuthResponse>(jsonAuthResponse)).Token;
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            // Return value is just for troubleshooting
            // If something fails in this method, the authToken should end up null, I think, based on how JsonConvert deserializes.
            return jsonAuthResponse;
        }

        public async Task<string> getHazardClasses()
        {
            return await jsonString(await jsonGET("emanifest/lookup/hazard-classes"));
        }

        // Pass null for the imgFilePath if there's no attachment
        public async Task<string> saveManifest(string jsonManifest, string imgFilePath)
        {
            if (imgFilePath != null && !File.Exists(imgFilePath))
            {
                throw new FileNotFoundException();
            }
            HttpResponseMessage resp = await eManifestAndAttachmentSave(jsonManifest, imgFilePath);
            return await jsonString(resp);
        }
        
        public async Task<string> eManifestAttachment(string trackingNumber, string fileSaveDirectoryPath)
        {
            if (!Directory.Exists(fileSaveDirectoryPath))
            {
                throw new DirectoryNotFoundException();
            }
            HttpResponseMessage resp = await eManifestAndAttachmentGET(trackingNumber);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                MultipartMemoryStreamProvider content = await resp.Content.ReadAsMultipartAsync();
                string jsonResponse = null;
                foreach (var c in content.Contents)
                {
                    if (c.Headers.ContentType.MediaType == "application/json")
                    {
                        jsonResponse = await c.ReadAsStringAsync();
                    }
                    else
                    {
                        using (FileStream fs = File.OpenWrite(Path.Combine(fileSaveDirectoryPath, "attachments.zip")))
                        using (Stream s = await c.ReadAsStreamAsync())
                        {
                            s.CopyTo(fs);
                        }

                    }
                }
                return jsonResponse;
            }
            else
            {
                return await resp.Content.ReadAsStringAsync();
            }

        }
        private async Task<string> jsonString(HttpResponseMessage resp)
        {
            return await resp.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> jsonGET(string endpoint)
        {
            HttpResponseMessage resp = null;
            for (int tries = 0; tries < 2; tries++)
            {
                resp = await _client.GetAsync(endpoint);
                if (tries == 0 && resp.StatusCode == HttpStatusCode.Forbidden)
                {
                    await authenticate();
                }
                else
                {
                    break;
                }
            }
            return resp;
        }
        private async Task<HttpResponseMessage> eManifestAndAttachmentSave(string jsonManifest, string imgFilePath)
        {
            HttpResponseMessage resp = null;
            for (int tries = 0; tries < 2; tries++)
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(jsonManifest), "manifest");
                    if (imgFilePath != null)
                    {
                        string imgFileName = Path.GetFileName(imgFilePath);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (ZipArchive z = new ZipArchive(ms, ZipArchiveMode.Create, true))
                            {
                                z.CreateEntryFromFile(imgFilePath, imgFileName, CompressionLevel.Optimal);
                            }
                            ms.Position = 0;
                            content.Add(new StreamContent(ms), "attachment", imgFileName);
                            resp = await _client.PostAsync("emanifest/manifest/save", content);
                            
                        }
                    }
                    else
                    {
                        resp = await _client.PostAsync("emanifest/manifest/save", content);
                    }
                    if (tries == 0 && resp.StatusCode == HttpStatusCode.Forbidden)
                    {
                        await authenticate();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return resp;
        }

        private async Task<HttpResponseMessage> eManifestAndAttachmentGET(string trackingNumber)
        {
            HttpResponseMessage resp = null;
            for (int tries = 0; tries < 2; tries++)
            {
                resp = await _client.GetAsync(String.Format("emanifest/manifest/{0}/attachments", trackingNumber));
                if (tries == 0 && resp.StatusCode == HttpStatusCode.Forbidden)
                {

                    await authenticate();
                }
                else
                {
                    break;
                }
            }
            return resp;
        }
        
    }
}
