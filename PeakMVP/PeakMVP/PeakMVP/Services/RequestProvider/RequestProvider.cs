using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.Contracts;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PeakMVP.Services.RequestProvider {
    public class RequestProvider : IRequestProvider {

        private readonly JsonSerializerSettings _serializerSettings;

        private readonly HttpClient _client;

        /// <summary>
        ///     ctor().
        /// </summary>
        public RequestProvider() {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Arrays
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public void ClientTokenReset() {
            if (_client != null)
                _client.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// POST.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request)
                where TRequest : class, IRequest
                where TResponse : class, IResponse, new() {
            return await Task.Run(async () => {
                TResponse responseToReturn = null;

                if (request is IAuthorization)
                    SetAccesToken(_client, ((IAuthorization)request).AccessToken);

                HttpContent content = null;

                if (request.Data is Dictionary<string, string>) {
                    Dictionary<string, string> keyValuePairs = request.Data as Dictionary<string, string>;
                    content = new FormUrlEncodedContent(keyValuePairs);
                }
                else {
                    content = new StringContent(JsonConvert.SerializeObject(request.Data));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                HttpResponseMessage httpResponseMessage = await _client.PostAsync(request.Url, content);

                await HandleResponse(httpResponseMessage);

                string json = await httpResponseMessage.Content.ReadAsStringAsync();

                responseToReturn = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(json, _serializerSettings));

                if (responseToReturn == null && httpResponseMessage.IsSuccessStatusCode) {
                    responseToReturn = new TResponse();
                }

                return responseToReturn;
            });
        }

        public async Task<TResponse> PatchAsync<TRequest, TResponse>(TRequest request)
                where TRequest : class, IRequest
                where TResponse : class, IResponse, new() {
            return await Task.Run(async () => {
                TResponse responseToReturn = null;

                if (request is IAuthorization) {
                    SetAccesToken(_client, ((IAuthorization)request).AccessToken);
                }

                //HttpContent content = null;

                //if (request.Data is Dictionary<string, string>) {
                //    Dictionary<string, string> keyValuePairs = request.Data as Dictionary<string, string>;
                //    content = new FormUrlEncodedContent(keyValuePairs);
                //}
                //else {
                //    content = new StringContent(JsonConvert.SerializeObject(request.Data));
                //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //}

                //HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), request));

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), request.Url);
                httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = await _client.SendAsync(httpRequestMessage);

                await HandleResponse(httpResponseMessage);

                string json = await httpResponseMessage.Content.ReadAsStringAsync();

                responseToReturn = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(json, _serializerSettings));

                if (responseToReturn == null && httpResponseMessage.IsSuccessStatusCode) {
                    responseToReturn = new TResponse();
                }

                return responseToReturn;
            });
        }

        /// <summary>
        /// PUT.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> PutAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse, new() {
            return await Task.Run(async () => {
                TResponse responseToReturn = null;

                if (request is IAuthorization)
                    SetAccesToken(_client, ((IAuthorization)request).AccessToken);

                HttpContent content = null;

                if (request.Data is Dictionary<string, string>) {
                    Dictionary<string, string> keyValuePairs = request.Data as Dictionary<string, string>;
                    content = new FormUrlEncodedContent(keyValuePairs);
                }
                else {
                    content = new StringContent(JsonConvert.SerializeObject(request.Data));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                HttpResponseMessage httpResponseMessage = await _client.PutAsync(request.Url, content);

                await HandleResponse(httpResponseMessage);

                string json = await httpResponseMessage.Content.ReadAsStringAsync();

                responseToReturn = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(json, _serializerSettings));

                if (responseToReturn == null && httpResponseMessage.IsSuccessStatusCode) {
                    responseToReturn = new TResponse();
                }

                return responseToReturn;
            });
        }

        /// <summary>
        /// GET.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request)
                where TRequest : class, IRequest
                where TResponse : class, IResponse =>
             await Task.Run(async () => {
                 TResponse responseToReturn = null;

                 if (request is IAuthorization)
                     SetAccesToken(_client, ((IAuthorization)request).AccessToken);

                 HttpResponseMessage httpResponseMessage = await _client.GetAsync(request.Url);

                 await HandleResponse(httpResponseMessage);

                 string json = httpResponseMessage.Content.ReadAsStringAsync().Result;

                 responseToReturn = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(json, _serializerSettings));

                 return responseToReturn;
             });

        /// <summary>
        /// Delete
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> DeleteAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse, new() {
            return await Task.Run(async () => {
                TResponse responseToReturn = null;

                if (request is IAuthorization)
                    SetAccesToken(_client, ((IAuthorization)request).AccessToken);

                HttpResponseMessage httpResponseMessage = await _client.DeleteAsync(request.Url);

                await HandleResponse(httpResponseMessage);

                string json = await httpResponseMessage.Content.ReadAsStringAsync();

                responseToReturn = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(json, _serializerSettings));

                if (responseToReturn == null && httpResponseMessage.IsSuccessStatusCode) {
                    responseToReturn = new TResponse();
                }

                return responseToReturn;
            });
        }

        private void SetAccesToken(HttpClient httpClient, string accesToken = "") {

            if (httpClient.DefaultRequestHeaders.Authorization == null) {
                if (!string.IsNullOrEmpty(accesToken)) {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                }
            }
            else {
                if (!(string.IsNullOrEmpty(accesToken)) && httpClient.DefaultRequestHeaders.Authorization.Parameter != accesToken) {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                }
            }
        }

        private async Task HandleResponse(HttpResponseMessage response) {
            if (!response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized) {

                    ClientTokenReset();

                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, string.IsNullOrEmpty(content) ? response.ReasonPhrase : content);
            }
        }
    }
}
