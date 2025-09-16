using System.Collections.Generic;

namespace KCBase.IDogCam.Models 
{
    public class RoomCameraConfiguration
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public string CameraId { get; set; }
        public string OtherServiceLink { get; set; }
        public bool EnabledForClient { get; set; }
        public string CurrentOccupant { get; set; } // "Smith: Rex"
        public string ExerciseServiceLink { get; set; }
        public KCBase.IDogCam.Models.ShowType ShowType { get; set; }  
        public List<TimeRange> AvailableTimes { get; set; }
        public string DisplayNameForClient {  get; set; }

    }
}
