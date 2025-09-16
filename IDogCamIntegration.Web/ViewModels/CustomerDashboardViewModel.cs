using KCBase.IDogCam.Models;
using System.Collections.Generic;

namespace IDogCamIntegration.Web.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public User CurrentUser { get; set; }
        public List<Pet> UserPets { get; set; } = new List<Pet>();
        public List<Camera> AccessibleCameras { get; set; } = new List<Camera>();
        public List<Appointment> CurrentAppointments { get; set; } = new List<Appointment>();
    }
}