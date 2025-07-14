// Example of direct database seeding (if needed)
using Microsoft.EntityFrameworkCore;
using MAPP.Modules.Observations.Infrastructure.Data;

var connectionString = "Host=localhost;Database=worldplanning;Username=mappDbUser;Password=gu3st123";

var options = new DbContextOptionsBuilder<ObservationsDbContext>()
    .UseNpgsql(connectionString)
    .Options;

using var context = new ObservationsDbContext(options);

// Your seeding logic here
// Read JSON file and create entities
// context.ObservationDomains.AddRange(domains);
// await context.SaveChangesAsync();

Console.WriteLine("Seeding completed!");
