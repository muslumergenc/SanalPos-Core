using IsBankMvc.Abstraction.Contracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace IsBankMvc.Abstraction.Implementation
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonService _jsonService;
        private readonly ILoggerService _logger;
        private string _token = string.Empty;

        public HttpService(ILoggerService logger, IJsonService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(ApplicationConstants.DefaultUserAgent);
        }

        public void ConfigureToken(string token, string? schema = null, string? header = null)
        {
            if (token == _token) return;

            _token = token;
            if (header is null)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(schema ?? "Bearer", _token);
                return;
            }

            _httpClient.DefaultRequestHeaders.Add(header, token);
        }

        public async Task<T?> Get<T>(string path, Dictionary<string, string> parameters)
        {
            try
            {
                var url = AddQueryString(path, parameters);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Get", e);
                return default;
            }
        }

        public void ConfigureEndpoint(string endpoint)
        {
            _httpClient.BaseAddress = new Uri(endpoint);
        }

        public async Task<string?> Submit(string path, object? payload)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(path, payload ?? new { });
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return content;
                throw new Exception(content);
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }
        public async Task<string?> SubmitRaw(string path, object payload)
        {
            try
            {
                var serialized = _jsonService.Serialize(payload);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(path, content);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }

        public async Task<T?> Post<T>(string path, object? payload)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(path, payload ?? new { });

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }

        public async Task<T?> PostFormEncoded<T>(string path, Dictionary<string, string> payload)
        {
            try
            {
                var encodedFormData = new FormUrlEncodedContent(payload);
                var response = await _httpClient.PostAsync(path, encodedFormData);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }

        public Task<T?> PostFormEncoded<T>(string path, object payload)
        {
            var dic = MapObjectToFormData(payload).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return PostFormEncoded<T>(path, dic);
        }

        public async Task<T?> PostRaw<T>(string path, object payload)
        {
            try
            {
                var serialized = _jsonService.Serialize(payload);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(path, content);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }


        public async Task<T?> Get<T>(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync(path);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Get", e);
                return default;
            }
        }

        public async Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage)
        {
            try
            {
                var response = await _httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T?>();
                return default;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, "HttpService.Post", e);
                return default;
            }
        }

        private static string AddQueryString(string url, Dictionary<string, string> parameters)
        {
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (var parameter in parameters)
                query[parameter.Key] = parameter.Value;

            builder.Query = query.ToString();
            return builder.ToString();
        }

        private static IEnumerable<KeyValuePair<string, string>> MapObjectToFormData(object obj, string prefix = null)
        {
            var formData = new List<KeyValuePair<string, string>>();

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var key = prefix != null ? $"{prefix}.{propertyInfo.Name}" : propertyInfo.Name;
                var value = propertyInfo.GetValue(obj);

                if (value != null)
                {
                    if (propertyInfo.PropertyType.Namespace!.Contains("ZarenTravel")) // Check for nested objects
                        formData.AddRange(MapObjectToFormData(value, key));
                    else
                        formData.Add(new KeyValuePair<string, string>(key, value.ToString()));
                }
            }

            return formData;
        }
    }
}
