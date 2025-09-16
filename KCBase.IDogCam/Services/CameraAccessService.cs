using KCBase.IDogCam.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class CameraAccessService : ICameraAccessService
{
    public bool CanEmployeeAccessCamera(string cameraId)
    {
        // Employees can always access any camera
        return !string.IsNullOrEmpty(cameraId);
    }

    public bool CanClientAccessCamera(string cameraId, CameraSettings settings)
    {
        if (string.IsNullOrEmpty(cameraId) || settings == null)
            return false;

        // Check run cameras
        var runCamera = settings.RunCameras?.FirstOrDefault(rc => rc.CameraId == cameraId);
        if (runCamera != null)
        {
            return runCamera.EnabledForClient;
        }

        // Check room cameras
        var roomCamera = settings.RoomCameras?.FirstOrDefault(rc => rc.CameraId == cameraId);
        if (roomCamera != null)
        {
            return CanAccessRoomCamera(roomCamera);
        }

        return false;
    }

    private bool CanAccessRoomCamera(RoomCameraConfiguration roomCamera)
    {
        var now = DateTime.Now.TimeOfDay;

        switch (roomCamera.ShowType)
        {
            case ShowType.AlwaysAvailable:
                return true;

            case ShowType.DuringBusinessHours:
                // Default business hours: 8 AM to 6 PM
                return now >= new TimeSpan(8, 0, 0) && now <= new TimeSpan(18, 0, 0);

            case ShowType.ScheduledTimes:
                return IsWithinAvailableTimes(currentTime: now, roomCamera.AvailableTimes);

            default:
                return false;
        }
    }

    private bool IsWithinAvailableTimes(TimeSpan currentTime, List<TimeRange> availableTimes)
    {
        if (availableTimes == null || !availableTimes.Any())
            return true; // No restrictions

        return availableTimes.Any(tr => currentTime >= tr.StartTime && currentTime <= tr.EndTime);
    }

    public string GetAccessDeniedReason(string cameraId, CameraSettings settings, bool isEmployee)
    {
        if (isEmployee)
        {
            return string.IsNullOrEmpty(cameraId) ? "Invalid camera ID" : "Camera temporarily unavailable";
        }

        if (string.IsNullOrEmpty(cameraId))
            return "Invalid camera ID";

        if (settings == null)
            return "Camera configuration not available";

        var runCamera = settings.RunCameras?.FirstOrDefault(rc => rc.CameraId == cameraId);
        if (runCamera != null)
        {
            if (!runCamera.EnabledForClient)
                return "Camera access is not enabled for clients";
        }

        var roomCamera = settings.RoomCameras?.FirstOrDefault(rc => rc.CameraId == cameraId);
        if (roomCamera != null)
        {
            var now = DateTime.Now.TimeOfDay;

            switch (roomCamera.ShowType)
            {
                case ShowType.DuringServiceTime:
                    if (now < new TimeSpan(8, 0, 0) || now > new TimeSpan(18, 0, 0))
                        return "Camera is only available during business hours (8 AM - 6 PM)";
                    break;

                //case ShowType.ScheduledTimes:
                //    if (!IsWithinAvailableTimes(now, roomCamera.AvailableTimes))
                //        return "Camera is not available at this time";
                //    break;
            }
        }

        return "Camera not found or access denied";
    }
}
