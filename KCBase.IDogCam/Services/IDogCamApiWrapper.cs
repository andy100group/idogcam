// Services/IDogCamApiWrapper.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KCBase.IDogCam.Models;
using System.Text.Json;

namespace KCBase.IDogCam.Services
{
    public interface IIDogCamApiService
    {
        Task<List<Camera>> GetCamerasAsync(IDogCamCredentials credentials);
        Task<bool> TestConnectionAsync(IDogCamCredentials credentials);
        string GetViewerUrl(string cameraId);
    }

    public class IDogCamApiService : IIDogCamApiService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_API_URL = "https://idogcam.com/idogcamkenconJSON.php";
        private const string VIEWER_BASE_URL = "https://idogcam.com/idogcamviewer.php?id=";

        public IDogCamApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<Camera>> GetCamerasAsync(IDogCamCredentials credentials)
        {
            try
            {
                if (credentials == null || string.IsNullOrEmpty(credentials.ApiKey) ||
                    string.IsNullOrEmpty(credentials.KennelId) || string.IsNullOrEmpty(credentials.ErpCode))
                {
                    throw new ArgumentException("Invalid credentials provided");
                }

                var requestUrl = $"{BASE_API_URL}?key={credentials.ApiKey}&kennelid={credentials.KennelId}&erpcode={credentials.ErpCode}";

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonContent))
                {
                    return new List<Camera>();
                }

                var cameras = JsonConvert.DeserializeObject<List<Camera>>(jsonContent);
                return cameras ?? new List<Camera>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to connect to iDogCam API: {ex.Message}", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new Exception($"Failed to parse camera data from iDogCam API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error calling iDogCam API: {ex.Message}", ex);
            }
        }

        public async Task<bool> TestConnectionAsync(IDogCamCredentials credentials)
        {
            try
            {
                var cameras = await GetCamerasAsync(credentials);
                return true; // If we get here without exception, connection is working
            }
            catch
            {
                return false;
            }
        }

        public string GetViewerUrl(string cameraId)
        {
            if (string.IsNullOrEmpty(cameraId))
            {
                throw new ArgumentException("Camera ID cannot be null or empty");
            }

            return $"{VIEWER_BASE_URL}{cameraId}";
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
