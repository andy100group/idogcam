using System.Collections.Generic;

namespace KCBase.IDogCam.Models 
{
    public class CameraSettings
    {
        public IDogCamCredentials Credentials { get; set; } = new IDogCamCredentials();
        public List<RunCameraConfiguration> RunCameras { get; set; } = new List<RunCameraConfiguration>();
        public List<RoomCameraConfiguration> RoomCameras { get; set; } = new List<RoomCameraConfiguration>();
    }
}
