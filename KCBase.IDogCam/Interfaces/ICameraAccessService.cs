using KCBase.IDogCam.Models;

public interface ICameraAccessService
{
    bool CanEmployeeAccessCamera(string cameraId);
    bool CanClientAccessCamera(string cameraId, CameraSettings settings);
    string GetAccessDeniedReason(string cameraId, CameraSettings settings, bool isEmployee);
}