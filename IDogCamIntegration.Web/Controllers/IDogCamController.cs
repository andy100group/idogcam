using IDogCamIntegration.Web.ViewModels;
using KCBase.IDogCam.Models;
using KCBase.IDogCam.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KCBase.IDogCam.Controllers
{
    public class IDogCamController : Controller
    {
        private readonly IIDogCamApiService _apiService;
        private readonly IConfigurationService _configService;
        private readonly IDummyDataService _dummyDataService;
        private readonly ICameraAccessService _accessService;

        public IDogCamController()
        {
            // In a real application, these would be injected via DI container
            _apiService = new MockIDogCamApiService(); // Use MockIDogCamApiService for development
            _configService = new XmlConfigurationService();
            _dummyDataService = new DummyDataService();
            _accessService = new CameraAccessService();
        }

        // GET: IDogCam/Setup
        public async Task<ActionResult> Setup()
        {
            var settings = _configService.LoadSettings();
            var viewModel = new CameraSetupViewModel
            {
                Credentials = settings.Credentials,
                RunCameras = settings.RunCameras,
                RoomCameras = settings.RoomCameras,
                AvailableRuns = _dummyDataService.GetRuns(),
                AvailableRooms = _dummyDataService.GetRooms(),
                AvailableServices = _dummyDataService.GetServices().Concat(_dummyDataService.GetExercises()).ToList()
            };

            // Try to load cameras if credentials exist
            if (!string.IsNullOrEmpty(settings.Credentials.ApiKey))
            {
                try
                {
                    viewModel.AvailableCameras = await _apiService.GetCamerasAsync(settings.Credentials);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Failed to load cameras: {ex.Message}";
                }
            }

            return View(viewModel);
        }

        // POST: IDogCam/SaveCredentials
        [HttpPost]
        public async Task<ActionResult> SaveCredentials(IDogCamCredentials credentials)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid credentials provided" });
            }

            try
            {
                // Test the connection
                var connectionTest = await _apiService.TestConnectionAsync(credentials);
                if (!connectionTest)
                {
                    return Json(new { success = false, message = "Failed to connect with provided credentials" });
                }

                // Save credentials
                var settings = _configService.LoadSettings();
                settings.Credentials = credentials;
                _configService.SaveSettings(settings);

                // Get cameras for the response
                var cameras = await _apiService.GetCamerasAsync(credentials);

                return Json(new
                {
                    success = true,
                    message = "Credentials saved successfully",
                    cameras = cameras.Select(c => new { c.Id, c.Title }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: IDogCam/SaveRunCamera
        [HttpPost]
        public ActionResult SaveRunCamera(RunCameraConfiguration runCamera)
        {
            try
            {
                var settings = _configService.LoadSettings();

                // Remove existing configuration for this run
                settings.RunCameras.RemoveAll(rc => rc.RunId == runCamera.RunId);

                // Add new configuration if camera is selected
                if (!string.IsNullOrEmpty(runCamera.CameraId))
                {
                    settings.RunCameras.Add(runCamera);
                }

                _configService.SaveSettings(settings);

                return Json(new { success = true, message = "Run camera configuration saved" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: IDogCam/SaveRoomCamera
        [HttpPost]
        public ActionResult SaveRoomCamera(RoomCameraConfiguration roomCamera)
        {
            try
            {
                var settings = _configService.LoadSettings();

                // Remove existing configuration for this room
                settings.RoomCameras.RemoveAll(rc => rc.RoomId == roomCamera.RoomId);

                // Add new configuration if camera is selected
                if (!string.IsNullOrEmpty(roomCamera.CameraId))
                {
                    settings.RoomCameras.Add(roomCamera);
                }

                _configService.SaveSettings(settings);

                return Json(new { success = true, message = "Room camera configuration saved" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: IDogCam/Viewer/{cameraId}
        public async Task<ActionResult>Viewer(string cameraId)
        {
            if (string.IsNullOrEmpty(cameraId))
            {
                return HttpNotFound("Camera ID is required");
            }

            var settings = _configService.LoadSettings();

            try
            {
                // Get camera details from API
                var cameras = await _apiService.GetCamerasAsync(settings.Credentials);
                var camera = cameras.FirstOrDefault(c => c.Id == cameraId);

                if (camera == null)
                {
                    return HttpNotFound("Camera not found");
                }

                var viewModel = new CameraViewerViewModel
                {
                    CameraId = cameraId,
                    Auth = camera.Auth,
                    CameraTitle = camera.Title
                    
                };
                               
                
                viewModel.CanView = _accessService.CanClientAccessCamera(cameraId, settings);
                viewModel.ReasonNotAvailable = viewModel.CanView ? null :
                    _accessService.GetAccessDeniedReason(cameraId, settings, false);
                

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var viewModel = new CameraViewerViewModel
                {
                    CameraId = cameraId,
                    CanView = false,
                    ReasonNotAvailable = $"Error loading camera: {ex.Message}"
                    
                };
                return View(viewModel);
            }
        }

        // GET: IDogCam/Test - Test page for employees
        public ActionResult Test()
        {
            try
            {
                var settings = _configService.LoadSettings();
                var runs = _dummyDataService.GetRuns();
                var rooms = _dummyDataService.GetRooms();

                ViewBag.Settings = settings ?? new CameraSettings();
                ViewBag.Runs = runs ?? new System.Collections.Generic.List<Run>();
                ViewBag.Rooms = rooms ?? new System.Collections.Generic.List<Room>();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading test data: {ex.Message}";
                ViewBag.Settings = new CameraSettings();
                ViewBag.Runs = new System.Collections.Generic.List<Run>();
                ViewBag.Rooms = new System.Collections.Generic.List<Room>();

                return View();
            }
        }

        // GET: IDogCam/ClientDemo - Demo page for client access
        public ActionResult ClientDemo()
        {
            var settings = _configService.LoadSettings();
            ViewBag.Settings = settings ?? new CameraSettings();

            return View();
        }


        [HttpGet]
        public ActionResult CameraProxy(string cameraId, string auth)
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    // Set user agent to avoid blocking
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                    var url = $"https://idogcam.com/idogcamviewer.php?id={Uri.EscapeDataString(cameraId)}&idogcamauth={Uri.EscapeDataString(auth)}";

                    // Download the content
                    var content = client.DownloadString(url);

                    // Replace the problematic streaming URLs to use our proxy
                    var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                    content = content.Replace("https://gracelanekennels.viewidogcam.com:10000/", baseUrl + "/IDogCam/StreamProxy/");

                    // Return as HTML content
                    Response.ContentType = "text/html";
                    return Content(content);
                }
            }
            catch (System.Net.WebException webEx)
            {
                var errorContent = $@"
        <html>
        <body style='font-family: Arial, sans-serif; padding: 20px; text-align: center;'>
            <h3 style='color: #e74c3c;'>Camera Service Error</h3>
            <p>Unable to connect to IDogCam service.</p>
            <p><strong>Details:</strong> {webEx.Message}</p>
            <p><a href='https://idogcam.com/idogcamviewer.php?id={Uri.EscapeDataString(cameraId)}&idogcamauth={Uri.EscapeDataString(auth)}' target='_blank'>Try Direct Link</a></p>
        </body>
        </html>";

                return Content(errorContent, "text/html");
            }
            catch (Exception ex)
            {
                var errorContent = $@"
        <html>
        <body style='font-family: Arial, sans-serif; padding: 20px; text-align: center;'>
            <h3 style='color: #e74c3c;'>Unexpected Error</h3>
            <p>An error occurred while loading the camera.</p>
            <p><strong>Error:</strong> {ex.Message}</p>
            <p><a href='https://idogcam.com/idogcamviewer.php?id={Uri.EscapeDataString(cameraId)}&idogcamauth={Uri.EscapeDataString(auth)}' target='_blank'>Try Direct Link</a></p>
        </body>
        </html>";

                return Content(errorContent, "text/html");
            }
        }

        [HttpGet]
        public ActionResult StreamProxy(string pathInfo = "")
        {
            try
            {
                // Reconstruct the original streaming URL
                var originalUrl = "https://gracelanekennels.viewidogcam.com:10000/" + pathInfo;
                if (!string.IsNullOrEmpty(Request.Url.Query))
                {
                    originalUrl += Request.Url.Query;
                }

                using (var client = new System.Net.WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                    // Determine content type based on file extension
                    string contentType = "application/octet-stream";
                    if (pathInfo.Contains("++hls_mediaplaylist"))
                    {
                        contentType = "application/vnd.apple.mpegurl"; // HLS playlist
                    }
                    else if (pathInfo.EndsWith(".ts"))
                    {
                        contentType = "video/mp2t"; // HLS segments
                    }
                    else if (pathInfo.EndsWith(".m3u8"))
                    {
                        contentType = "application/vnd.apple.mpegurl";
                    }

                    var data = client.DownloadData(originalUrl);

                    // Add CORS headers
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                    return File(data, contentType);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content($"Stream proxy error: {ex.Message}");
            }
        }
    }
}