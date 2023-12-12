using AirportAutomationWeb.Authentication;
using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace AirportAutomationWeb.Services
{
	public class HttpCallService : IHttpCallService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;
		private readonly ILogger<HttpCallService> _logger;
		private readonly string apiURL;

		public HttpCallService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration, ILogger<HttpCallService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
			_logger = logger;
			apiURL = _configuration.GetValue<string>("ApiSettings:apiUrl");
		}

		// Send Data to Authenticate
		public async Task<bool> Authenticate(User user)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				return true;
			}

			var bearerToken = string.Empty;
			var requestUri = $"{apiURL}/Authentication";

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			var response = await httpClient.PostAsJsonAsync(requestUri, user).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				bearerToken += await response.Content.ReadFromJsonAsync<string>().ConfigureAwait(false);
				_httpContextAccessor.HttpContext.Session.SetString("AccessToken", bearerToken);
				return true;
			}
			else if (((int)response.StatusCode) == 400 || ((int)response.StatusCode) == 401)
			{
				_logger.LogError("Unauthorized");
			}
			else
			{
				_logger.LogError("Failed to authenticate. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}

		// Add Bearer Token To Headers For Authorization
		private void AddAuthorizationHeader(HttpClient httpClient)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				_logger.LogError("Bearer token is missing or invalid.");
			}
		}

		// Retrieve Token From Session
		public string GetToken()
		{
			string token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
			if (!string.IsNullOrEmpty(token))
			{
				return token;
			}
			else
			{
				_logger.LogError("Bearer token is missing or invalid.");
			}
			return string.Empty;
		}

		// Remove Token From Session For Sign Out
		public bool RemoveToken()
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				_httpContextAccessor.HttpContext.Session.Remove("AccessToken");
				_httpContextAccessor.HttpContext.Session.CommitAsync().Wait();
				return true;
			}
			return false;
		}

		// Get Data With Pagination
		public async Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize)
		{
			var model = typeof(T);
			var requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/";
			}
			else
			{
				requestUri += $"s/";
			}
			requestUri = requestUri + $"?page={page}&pageSize={pageSize}";

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogError("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		// Get Data By Id
		public async Task<T> GetData<T>(int id)
		{
			T data = default;
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/{id}";
			}
			else
			{
				requestUri += $"s/{id}";
			}

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return data;
		}

		// Get Data And Use JS Ajax To Show On WEB
		public async Task<PagedResponse<T>> GetDataList<T>()
		{
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += "es";
			}
			else
			{
				requestUri += "s";
			}

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		// Get Data By Name - JSON
		public async Task<string> GetDataByName<T>(string name)
		{
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/byName/{name}";
			}
			else
			{
				requestUri += $"s/byName/{name}";
			}

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		// Get Data By First Name or Last Name - JSON
		public async Task<string> GetDataByFNameOrLName<T>(string firstName, string lastName)
		{
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/byName";
			}
			else
			{
				requestUri += $"s/byName/";
			}
			UriBuilder uriBuilder = new UriBuilder(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(firstName))
			{
				query["firstName"] = firstName;
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query["lastName"] = lastName;
			}
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		// Get Data By Price - JSON
		public async Task<string> GetDataForPrice<T>(int? minPrice, int? maxPrice)
		{
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/byPrice";
			}
			else
			{
				requestUri += $"s/byPrice/";
			}
			UriBuilder uriBuilder = new UriBuilder(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (minPrice != null)
			{
				query["minPrice"] = minPrice.ToString();
			}
			if (maxPrice != null)
			{
				query["maxPrice"] = maxPrice.ToString();
			}
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		// Get Data Between Dates - JSON
		public async Task<string> GetDataBetweenDates<T>(string startDate, string endDate)
		{
			var model = typeof(T);

			string requestUri = $"{apiURL}/{model.Name}";
			if (model.Name.Equals("TravelClass"))
			{
				requestUri += $"es/byDate";
			}
			else
			{
				requestUri += $"s/byDate/";
			}
			UriBuilder uriBuilder = new UriBuilder(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(startDate))
			{
				query["startDate"] = startDate;
			}
			if (!string.IsNullOrEmpty(endDate))
			{
				query["endDate"] = endDate;
			}
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				_logger.LogError("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		public async Task<T> CreateData<T>(T t)
		{
			T data = default;
			var model = typeof(T);
			string requestUri = $"{apiURL}/{model.Name}s";

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.PostAsJsonAsync(requestUri, t).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogError("Failed to create data. Status code: {StatusCode}", response.StatusCode);
			}
			return data;
		}

		public async Task<bool> EditData<T>(T t, int id)
		{
			var model = typeof(T);
			string requestUri = $"{apiURL}/{model.Name}s/{id}";

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.PutAsJsonAsync(requestUri, t);

			if (response.StatusCode is HttpStatusCode.NoContent)
			{
				return true;
			}
			else
			{
				_logger.LogError("Failed to edit data. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}

		public async Task<bool> DeleteData<T>(int id)
		{
			var model = typeof(T);
			var requestUri = $"{apiURL}/{model.Name}s/{id}";

			var httpClient = _httpClientFactory.CreateClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
			var response = await httpClient.DeleteAsync(requestUri);

			if (response.StatusCode is HttpStatusCode.NoContent)
			{
				return true;
			}
			else if (response.StatusCode is HttpStatusCode.Conflict)
			{
				return false;
			}
			else
			{
				_logger.LogError("Failed to delete data. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}
	}
}
