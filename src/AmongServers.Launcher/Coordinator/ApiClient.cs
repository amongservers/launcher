using AmongServers.Launcher.Coordinator.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AmongServers.Launcher
{
    /// <summary>
    /// Provides functionality to request data from the API.
    /// </summary>
    public class ApiClient
    {
        private HttpClient _client;

        /// <summary>
        /// The number of retries when the service is unavailable.
        /// </summary>
        public int Retries { get; set; } = 3;

        /// <summary>
        /// The timeout for requests.
        /// </summary>
        public TimeSpan Timeout {
            get {
                return _client.Timeout;
            } set {
                _client.Timeout = value;
            }
        }

        /// <summary>
        /// Gets the servers(s) from the REST API.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServerEntity[]> ListServersAsync(CancellationToken cancellationToken = default)
        {
            int retryIndex = 0;

            while (retryIndex != Retries + 1) {
                HttpResponseMessage responseMessage = await _client.GetAsync($"server", cancellationToken);
                retryIndex++;

                if (responseMessage.IsSuccessStatusCode) {
                    string str = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServerEntity[]>(str);
                } else if (responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable) {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    continue;
                } else {
                    throw new Exception($"The service returned an error: {responseMessage.StatusCode}");
                }
            }

            throw new Exception("The service is unavailable, try again later");
        }

        /// <summary>
        /// Gets the banner(s) from the REST API, should either contain the project link or a link to update if the provided version is out of date.
        /// </summary>
        /// <param name="version">The application version.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BannerEntity[]> ListBannersAsync(string version, CancellationToken cancellationToken = default)
        {
            int retryIndex = 0;

            while (retryIndex != Retries + 1) {
                HttpResponseMessage responseMessage = await _client.GetAsync($"banner?version={HttpUtility.UrlEncode(version)}", cancellationToken);
                retryIndex++;

                if (responseMessage.IsSuccessStatusCode) {
                    string str = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BannerEntity[]>(str);
                } else if (responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable) {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    continue;
                } else {
                    throw new Exception($"The service returned an error: {responseMessage.StatusCode}");
                }
            }

            throw new Exception("The service is unavailable, try again later");
        }

        /// <summary>
        /// Creates an API client.
        /// </summary>
        /// <param name="apiUrl">The API client.</param>
        public ApiClient(string apiUrl)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(apiUrl);
        }
    }
}
