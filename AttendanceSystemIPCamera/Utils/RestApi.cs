using AttendanceSystemIPCamera.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Utils
{
    public class RestApi
    {

        public static async Task<T> PostAsync<T>(string url, object requestMessage) where T : class
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestMessage),
                                            Encoding.UTF8, "application/json");
            var response = await client.PostAsync(new Uri(url), content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonConvert.DeserializeObject<BaseResponse<T>>(responseData);
                if (baseResponse.Success)
                {
                    return baseResponse.Data;
                }
            }
            return null;
        }
        public static async Task<T> GetAsync<T>(string url) where T : class
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(new Uri(url));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonConvert.DeserializeObject<BaseResponse<T>>(responseData);
                if (baseResponse.Success)
                {
                    return baseResponse.Data;
                }
            }
            return null;
        }
        public static async Task<Stream> GetContentStreamAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(new Uri(url));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
                return contentStream;
            }
            return null;
        }

    }
}
