using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCBase.IDogCam.Models
{
    // iDogCam API Models
    public class Camera
    {
        public string Id { get; set; }
        public string KennelId { get; set; }
        public string Client { get; set; }
        public string Title { get; set; }
        public string Auth { get; set; }
    }
}