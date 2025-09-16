using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using KCBase.IDogCam.Models;

namespace KCBase.IDogCam.Services
{
    public interface IConfigurationService
    {
        CameraSettings LoadSettings();
        void SaveSettings(CameraSettings settings);
    }

    public class XmlConfigurationService : IConfigurationService
    {
        private readonly string _configPath;

        public XmlConfigurationService()
        {
            _configPath = HttpContext.Current?.Server.MapPath("~/App_Data/idogcam_settings.xml")
                         ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "idogcam_settings.xml");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public CameraSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    return new CameraSettings();
                }

                var serializer = new XmlSerializer(typeof(CameraSettings));
                using (var reader = new FileStream(_configPath, FileMode.Open))
                {
                    return (CameraSettings)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                // Log error in production
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return new CameraSettings();
            }
        }

        public void SaveSettings(CameraSettings settings)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(CameraSettings));
                using (var writer = new FileStream(_configPath, FileMode.Create))
                {
                    serializer.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                // Log error in production
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
                throw;
            }
        }
    }
}