using System;
using System.Collections.Generic;

namespace KCBase.IDogCam.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int PetId { get; set; }
        public string PetName { get; set; }
        public string RunId { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public List<Service> Exercises { get; set; } = new List<Service>();
    }
}
