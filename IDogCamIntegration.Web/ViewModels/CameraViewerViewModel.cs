
namespace IDogCamIntegration.Web.ViewModels 
{
    public class CameraViewerViewModel
    {
        public string CameraId { get; set; }
        public string Auth { get; set; }
        public string CameraTitle { get; set; }
        public bool CanView { get; set; }
        public string ReasonNotAvailable { get; set; }
        public int? PetId { get; set; }
        public bool IsEmployeeView { get; set; }
    }
}
