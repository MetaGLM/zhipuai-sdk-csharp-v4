using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZhipuApi.Models;
using ZhipuApi.Utils;
using System.Diagnostics;
using ZhipuApi.Models.RequestModels;
using ZhipuApi.Models.ResponseModels.ImageGenerationModels;

namespace ZhipuApi.Modules
{
    public class Images
    {
        private string _apiKey;
        private static readonly int API_TOKEN_TTL_SECONDS = 60 * 5;
        static readonly HttpClient client = new HttpClient();

        public Images(string apiKey)
        {
            this._apiKey = apiKey;
        }

        private IEnumerable<string> GenerateBase(ImageRequestBase requestBody)
        {
            // Console.WriteLine("----1----");
            var json = JsonSerializer.Serialize(requestBody);
            // Console.WriteLine(json);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var api_key = AuthenicationUtils.GenerateToken(this._apiKey, API_TOKEN_TTL_SECONDS);
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://open.bigmodel.cn/api/paas/v4/images/generations"),
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
        
        public ImageResponseBase Generation(ImageRequestBase requestBody)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var str in GenerateBase(requestBody))
            {
                sb.Append(str);
            }
            // Console.WriteLine(sb.ToString());

            return ImageResponseBase.FromJson(sb.ToString());
        }

    }
}