// Controllers/IDogCamController.cs
using IDogCamIntegration.Web.ViewModels;
using KCBase.IDogCam.Models;
using KCBase.IDogCam.Services;
using System;
using System.Collections.Generic;
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
            _accessService = new CameraAccessService(_dummyDataService);
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

        // GET: IDogCam/Viewer/{cameraId}?petId={petId}&employee={true/false}
        public async Task<ActionResult> Viewer(string cameraId, int? petId = null, bool employee = false)
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
                    CameraTitle = camera.Title,
                    PetId = petId,
                    IsEmployeeView = employee
                };

                // Check access permissions
                if (employee)
                {
                    viewModel.CanView = _accessService.CanEmployeeAccessCamera(cameraId);
                    viewModel.ReasonNotAvailable = viewModel.CanView ? null : "Camera not available";
                }
                else if (petId.HasValue)
                {
                    viewModel.CanView = _accessService.CanPetAccessCamera(petId.Value, cameraId, settings);
                    viewModel.ReasonNotAvailable = viewModel.CanView ? null :
                        _accessService.GetAccessDeniedReason(petId.Value, cameraId, settings);
                }
                else
                {
                    viewModel.CanView = false;
                    viewModel.ReasonNotAvailable = "Pet ID is required for client access";
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var viewModel = new CameraViewerViewModel
                {
                    CameraId = cameraId,
                    CanView = false,
                    ReasonNotAvailable = $"Error loading camera: {ex.Message}",
                    PetId = petId,
                    IsEmployeeView = employee
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
                var appointments = _dummyDataService.GetCurrentAppointments();
                var pets = _dummyDataService.GetPets();

                // Ensure collections are not null
                ViewBag.Settings = settings ?? new CameraSettings();
                ViewBag.Appointments = appointments ?? new List<Appointment>();
                ViewBag.Pets = pets ?? new List<Pet>();

                return View();
            }
            catch (Exception ex)
            {
                // Log the error (in production, use proper logging)
                ViewBag.ErrorMessage = $"Error loading test data: {ex.Message}";
                ViewBag.Settings = new CameraSettings();
                ViewBag.Appointments = new List<Appointment>();
                ViewBag.Pets = new List<Pet>();

                return View();
            }
        }

        // GET: IDogCam/PetView/{petId} - Test page for pet owners
        public ActionResult PetView(int petId)
        {
            try
            {
                var pet = _dummyDataService.GetPets().FirstOrDefault(p => p.Id == petId);
                if (pet == null)
                {
                    return HttpNotFound("Pet not found");
                }

                var appointment = _dummyDataService.GetAppointmentByPetId(petId);
                var settings = _configService.LoadSettings();

                ViewBag.Pet = pet;
                ViewBag.Appointment = appointment; // This can be null if pet not checked in
                ViewBag.Settings = settings ?? new CameraSettings();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading pet data: {ex.Message}";
                return View("Error");
            }
        }

        // GET: IDogCam/GetRunOccupant/{runId} - AJAX endpoint to get current occupant
        [HttpGet]
        public ActionResult GetRunOccupant(string runId)
        {
            var appointment = _dummyDataService.GetCurrentAppointments()
                .FirstOrDefault(a => a.RunId == runId);

            if (appointment != null)
            {
                var pet = _dummyDataService.GetPets().FirstOrDefault(p => p.Id == appointment.PetId);
                if (pet != null)
                {
                    return Json(new { occupant = $"{pet.OwnerName}: {pet.Name}" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { occupant = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}