using System;

namespace KCBase.IDogCam.Models
{
    public class UserCameraAccess
    {
        public int UserId { get; set; }
        public string CameraId { get; set; }
        public string CameraName { get; set; }
        public DateTime GrantedDate { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public bool IsActive { get; set; }
        public string GrantedBy { get; set; }
    }
}
