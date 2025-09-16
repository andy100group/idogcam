using KCBase.IDogCam.Models;
using System.Collections.Generic;

public class DummyDataService : IDummyDataService
{
    private static readonly List<Service> _services = new List<Service>
        {
            new Service { Id = 1, ServiceName = "Cam Service", Price = 15.00m },
            new Service { Id = 2, ServiceName = "Premium Boarding", Price = 45.00m },
            new Service { Id = 3, ServiceName = "Daycare", Price = 30.00m },
            new Service { Id = 4, ServiceName = "Grooming", Price = 60.00m }
        };

    private static readonly List<Service> _exercises = new List<Service>
        {
            new Service { Id = 10, ServiceName = "Play Time", Price = 20.00m },
            new Service { Id = 11, ServiceName = "Swimming", Price = 25.00m },
            new Service { Id = 12, ServiceName = "Individual Walk", Price = 15.00m },
            new Service { Id = 13, ServiceName = "Group Play", Price = 18.00m }
        };

    private static readonly List<Run> _runs = new List<Run>
        {
            new Run { Id = "R001", Name = "Large 10", Size = "Large" },
            new Run { Id = "R002", Name = "Large 11", Size = "Large" },
            new Run { Id = "R003", Name = "Medium 5", Size = "Medium" },
            new Run { Id = "R004", Name = "Medium 6", Size = "Medium" },
            new Run { Id = "R005", Name = "Small 1", Size = "Small" },
            new Run { Id = "R006", Name = "Small 2", Size = "Small" }
        };

    private static readonly List<Room> _rooms = new List<Room>
        {
            new Room { Id = "YARD1", Name = "Doggy Play Yard", Type = "Yard" },
            new Room { Id = "POOL1", Name = "Swimming Pool Area", Type = "Pool" },
            new Room { Id = "LOBBY", Name = "Main Lobby", Type = "Common" },
            new Room { Id = "KITCHEN", Name = "Feeding Area", Type = "Feeding" }
        };

    public List<Service> GetServices() => _services;
    public List<Service> GetExercises() => _exercises;
    public List<Run> GetRuns() => _runs;
    public List<Room> GetRooms() => _rooms;
}
