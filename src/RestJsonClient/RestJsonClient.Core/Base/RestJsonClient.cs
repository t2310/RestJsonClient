using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestJson.Core.Base
{
    public abstract class RestJsonClient
    {
        protected readonly JsonSerializerSettings _jsonSerializerSettings;

        protected readonly string _baseUrl;
        protected readonly string _contentType;

        protected readonly string _getMethod;
        protected readonly string _postMethod;
        protected readonly string _putMethod;

        protected RestJsonClient(string baseUrl, JsonSerializerSettings jsonSerializerSettings = null)
        {
            _baseUrl = baseUrl;
            _contentType = "application/json";

            _getMethod = "GET";
            _postMethod = "POST";
            _putMethod = "PUT";

            _jsonSerializerSettings = jsonSerializerSettings;

            if (_jsonSerializerSettings == null)
            {
                _jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };
            }
        }

        public async Task<TK> Post<T, TK>(T body, string method)
        {
            TK answer;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_baseUrl + method);
            httpWebRequest.ContentType = _contentType;
            httpWebRequest.Method = _postMethod;

            var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync());

            var json = JsonConvert.SerializeObject(body, _jsonSerializerSettings);

            streamWriter.Write(json);
            streamWriter.Flush();

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var response = streamReader.ReadToEnd();
                answer = JsonConvert.DeserializeObject<TK>(response);
            }

            return answer;
        }

        public async Task<T> Get<T>(string method)
        {
            T answer;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_baseUrl + method);
            httpWebRequest.ContentType = _contentType;
            httpWebRequest.Method = _getMethod;

            HttpWebResponse httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var response = streamReader.ReadToEnd();
                answer = JsonConvert.DeserializeObject<T>(response);
            }

            return answer;
        }
    }
}
