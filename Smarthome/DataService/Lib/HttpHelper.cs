using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace smarthome.DataService.Lib
{
    public class HttpHelper
    {
        private string _baseApiAddress { get; set; }
        public HttpHelper(string baseApiUrl)
        {
            this._baseApiAddress = baseApiUrl;
        }

        public T DoGet<T>(string apiUri)
        {
            HttpResponseMessage httpResponse = DoHttpGet(apiUri);

            if (httpResponse == null)
                throw new ApplicationException("Unexpected error occured. HttpResponse null.");

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new KeyNotFoundException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase);
                else
                    throw new ApplicationException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase);
            }

            var rawData = httpResponse.Content.ReadAsStringAsync().Result;

            var data = JsonConvert.DeserializeObject<T>(rawData.ToString());

            if (data == null)
                throw new ApplicationException("Unexpected error occured. JsonConvert.DeserializeObject<T>(rawData.ToString() == null");

            return data;
        }

        public T DoPatch<T>(string apiUri, string jsonPostBodyString)
        {
            StringContent content = new StringContent(jsonPostBodyString, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = DoHttpPatch(apiUri, content);

            if (httpResponse == null)
                throw new ApplicationException("Unexpected error occured. HttpResponse null.");

            var rawData = httpResponse.Content.ReadAsStringAsync().Result;

            if (!httpResponse.IsSuccessStatusCode)
                throw new ApplicationException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase + " " + rawData);

            var data = JsonConvert.DeserializeObject<T>(rawData.ToString());

            if (data == null)
                throw new ApplicationException("Unexpected error occured. JsonConvert.DeserializeObject<T>(rawData.ToString() == null");

            return data;
        }

        public T DoPost<T>(string apiUri, string jsonPostBodyString)
        {
            StringContent content = new StringContent(jsonPostBodyString, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = DoHttpPost(apiUri, content);

            if (httpResponse == null)
                throw new ApplicationException("Unexpected error occured. HttpResponse null.");

            if (!httpResponse.IsSuccessStatusCode)
                throw new ApplicationException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase);

            var rawData = httpResponse.Content.ReadAsStringAsync().Result;

            var data = JsonConvert.DeserializeObject<T>(rawData.ToString());

            if (data == null)
                throw new ApplicationException("Unexpected error occured. JsonConvert.DeserializeObject<T>(rawData.ToString() == null");

            return data;
        }

        public T DoPut<T>(string apiUri, string jsonPostBodyString)
        {
            StringContent content = new StringContent(jsonPostBodyString, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = DoHttpPut(apiUri, content);

            if (httpResponse == null)
                throw new ApplicationException("Unexpected error occured. HttpResponse null.");

            var rawData = httpResponse.Content.ReadAsStringAsync().Result;

            if (!httpResponse.IsSuccessStatusCode)
                throw new ApplicationException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase + " " + rawData);

            var data = JsonConvert.DeserializeObject<T>(rawData.ToString());

            if (data == null)
                throw new ApplicationException("Unexpected error occured. JsonConvert.DeserializeObject<T>(rawData.ToString() == null");

            return data;
        }

        public T DoDelete<T>(string apiUri)
        {
            //StringContent content = new StringContent(jsonPostBodyString, Encoding.UTF8, ConfigHelper.Ua.Api.ContentTypeValue.Value);

            HttpResponseMessage httpResponse = DoHttpDelete(apiUri);

            if (httpResponse == null)
                throw new ApplicationException("Unexpected error occured. HttpResponse null.");

            var rawData = httpResponse.Content.ReadAsStringAsync().Result;

            if (!httpResponse.IsSuccessStatusCode)
                throw new ApplicationException(httpResponse.StatusCode.ToString() + " " + httpResponse.ReasonPhrase + " " + rawData);

            var data = JsonConvert.DeserializeObject<T>(rawData.ToString());

            if (data == null)
                throw new ApplicationException("Unexpected error occured. JsonConvert.DeserializeObject<T>(rawData.ToString() == null");

            return data;
        }

        private HttpResponseMessage DoHttpGet(string uriPath)
        {
            HttpClient client = BuildHttpClient();
            HttpResponseMessage response = client.GetAsync(uriPath).Result;
            return response;
        }

        private HttpResponseMessage DoHttpPost(string uriPath, StringContent content)
        {
            HttpClient client = BuildHttpClient();
            HttpResponseMessage response = client.PostAsync(uriPath, content).Result;
            return response;
        }

        private HttpResponseMessage DoHttpPatch(string uriPath, StringContent content)
        {
            HttpClient client = BuildHttpClient();
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, uriPath)
            {
                Content = content,
            };
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }

        private HttpResponseMessage DoHttpPut(string uriPath, StringContent content)
        {
            HttpClient client = BuildHttpClient();
            var method = new HttpMethod("PUT");
            var request = new HttpRequestMessage(method, uriPath)
            {
                Content = content,
            };
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }

        private HttpResponseMessage DoHttpDelete(string uriPath)
        {
            HttpClient client = BuildHttpClient();
            HttpResponseMessage response = client.DeleteAsync(uriPath).Result;
            return response;
        }

        private HttpClient BuildHttpClient()
        {
            HttpClient client = new HttpClient();

            //string baseApiAddress = this._baseApiAddress;
                //ConfigHelper.DocumentGenerator.Api.Scheme.Value +
                //ConfigHelper.DocumentGenerator.Api.HostName.Value +
                //ConfigHelper.DocumentGenerator.Api.DelimiterCharacter.Value;

            client.BaseAddress = new Uri(_baseApiAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add(ConfigHelper.DocumentGenerator.Api.AuthorizationLabel.Value, ConfigHelper.DocumentGenerator.Api.AuthorizationValue.Value);
            return client;
        }
    }
}