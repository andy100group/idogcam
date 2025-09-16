using KCBase.IDogCam.Models;
using System.Collections.Generic;

public interface IDummyDataService
{
    List<Service> GetServices();
    List<Service> GetExercises();
    List<Run> GetRuns();
    List<Room> GetRooms();
}