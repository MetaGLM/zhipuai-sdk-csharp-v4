using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZhipuApi.Models;
using ZhipuApi.Utils;
using System.Diagnostics;
using Newtonsoft.Json;
using ZhipuApi.Models.RequestModels;
using ZhipuApi.Models.ResponseModels;
using ZhipuApi.Utils.JsonResolver;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ZhipuApi.Modules
{
    public enum ModelPortal
    {
        Regular,
        Character,
    }

    public class Chat
    {
        
        private string _apiKey;
        private static readonly int API_TOKEN_TTL_SECONDS = 60 * 5;
        
        static readonly HttpClient client = new HttpClient();

        private static readonly Dictionary<ModelPortal, string> PORTAL_URLS = new Dictionary<ModelPortal, string>
        {
            { ModelPortal.Regular , "https://open.bigmodel.cn/api/paas/v4/chat/completions"},
        };

        
        public Chat(string apiKey)
        {
            this._apiKey = apiKey;
        }

        private IEnumerable<string> CompletionBase(TextRequestBase textRequestBody)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonResolver(),
                Formatting = Formatting.Indented
            };
            
            var json = JsonConvert.SerializeObject(textRequestBody, settings);
            // Console.WriteLine("----1----");
            // Console.WriteLine(json);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var api_key = AuthenicationUtils.GenerateToken(this._apiKey, API_TOKEN_TTL_SECONDS);
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://open.bigmodel.cn/api/paas/v4/chat/completions"),
                Content = data,
                Headers =
                {
                    { "Authorization", api_key }
                },
                
            };

            var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            var stream = response.Content.ReadAsStreamAsync().Result;
            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                yield return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        public ResponseBase Completion(TextRequestBase textRequestBody)
        {
            textRequestBody.stream = false;
            StringBuilder sb = new StringBuilder();
            foreach (var str in CompletionBase(textRequestBody))
            {
                sb.Append(str);
            }

            // Console.WriteLine(sb.ToString());
            
            return ResponseBase.FromJson(sb.ToString());
        }
        
        public IEnumerable<ResponseBase> Stream(TextRequestBase textRequestBody)
        {
            textRequestBody.stream = true;
            var buffer = string.Empty;
            foreach (var chunk in CompletionBase(textRequestBody))
            {
                
                buffer += chunk;
                
                // Console.WriteLine("-----buffer-----");
                // Console.WriteLine(buffer);
                // Console.WriteLine("----------------");
                
                // Keep checking the buffer to see if we have received a complete JSON
                while (true)
                {
                    int startPos = buffer.IndexOf("data: ", StringComparison.Ordinal);
                    if (startPos == -1)
                    {
                        break;
                    }
                        
                    int endPos = buffer.IndexOf("\n\n", startPos, StringComparison.Ordinal);
                    
                    if (endPos == -1)
                    {
                        // We don't have a complete JSON in our buffer yet, break the loop until we get more chunks
                        break;
                    }
            
                    // Calculate the starting position of the actual JSON data after "data: " and "\n\n"
                    startPos += "data: ".Length;
            
                    // We have a complete JSON, extract it, remove the "data: " prefix and yield return it
                    string jsonString = buffer.Substring(startPos, endPos - startPos);
                    // Console.WriteLine(">>" + jsonString);
                    if (jsonString.Equals("[DONE]"))
                    {
                        break;
                    }
                    
                    var response = ResponseBase.FromJson(jsonString);
                    if (response != null)
                    {
                        yield return response;
                    }
                     
                    // Remove the processed JSON plus two "\n"s from our buffer
                    buffer = buffer.Substring(endPos + "\n\n".Length);
                }
                
            }
            
            var finalResponse = ResponseBase.FromJson(buffer.Trim());
            if (finalResponse != null)
            {
                yield return finalResponse;
            }
            
            // Console.WriteLine("-----end-buffer-----");
            // Console.WriteLine(buffer);
            // Console.WriteLine("--------------------");
        }
    }
}