using AttendanceSystemIPCamera.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Utils
{
    public class RestApi
    {

        public static async Task<T> CallApiAsync<T>(string url, object requestMessage) where T : class
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestMessage), 
                                            Encoding.UTF8, "application/json");
            var response = await client.PostAsync(new Uri(url), content);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
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

    }
}
