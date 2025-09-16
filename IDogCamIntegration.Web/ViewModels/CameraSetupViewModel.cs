using KCBase.IDogCam.Models;
using System.Collections.Generic;

namespace IDogCamIntegration.Web.ViewModels 
{
    public class CameraSetupViewModel
    {
        public IDogCamCredentials Credentials { get; set; } = new IDogCamCredentials();
        public List<RunCameraConfiguration> RunCameras { get; set; } = new List<RunCameraConfiguration>();
        public List<RoomCameraConfiguration> RoomCameras { get; set; } = new List<RoomCameraConfiguration>();
        public List<Camera> AvailableCameras { get; set; } = new List<Camera>();
        public List<Run> AvailableRuns { get; set; } = new List<Run>();
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
        public List<Service> AvailableServices { get; set; } = new List<Service>();
    }
}
