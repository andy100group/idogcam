// Mock service for development/testing when API is not available
using KCBase.IDogCam.Models;
using KCBase.IDogCam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MockIDogCamApiService : IIDogCamApiService
{
    public async Task<List<Camera>> GetCamerasAsync(IDogCamCredentials credentials)
    {
        // Simulate API delay
        await Task.Delay(500);

        return new List<Camera>
            {
                new Camera { Id = "12732", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Suite 401 Indoor", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" },
                new Camera { Id = "12733", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Suite 402 Indoor", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" },
                new Camera { Id = "12734", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Suite 403 Indoor", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" },
                new Camera { Id = "12735", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Suite 404 Indoor", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" },
                new Camera { Id = "12736", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Doggy Play Yard", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" },
                new Camera { Id = "12737", KennelId = "14001", Client = "iDogCam Demo Kennel", Title = "Swimming Pool Area", Auth = "cGF3cGF3NjQ6cGF3cGF3NjQ=" }
            };
    }

    public async Task<bool> TestConnectionAsync(IDogCamCredentials credentials)
    {
        await Task.Delay(100);
        return !string.IsNullOrEmpty(credentials?.ApiKey);
    }

    public string GetViewerUrl(string cameraId)
    {
        return $"https://idogcam.com/idogcamviewer.php?id={cameraId}";
    }
}