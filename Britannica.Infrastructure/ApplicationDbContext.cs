using Britannica.Domain.Bases;
using Britannica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace Britannica.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {

        private readonly string _dbPath;
        public ApplicationDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            var configurationDbPath = configuration["DbPath"];
            if (string.IsNullOrWhiteSpace(configurationDbPath))
                throw new ArgumentNullException(nameof(configurationDbPath));
            _dbPath = configurationDbPath;
        }
        public DbSet<FlightEntity> Flights { get; set; }
        public DbSet<SeatEntity> Seats { get; set; }
        public DbSet<PassengerEntity> Passengers { get; set; }
        public DbSet<PassengerFlightEntity> PassengerFlights { get; set; }
        public DbSet<AircraftEntity> Aircrafts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={_dbPath}");

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var created = new DateTime(2021, 08, 21, 20, 41, 00, DateTimeKind.Utc);

            builder.Entity<PassengerEntity>().HasData(new PassengerEntity
            {
                Id = 1,
                Created = created,
                FirtName = "Avi",
                LastName = "Nessimian",
            });

            builder.Entity<PassengerEntity>().HasData(new PassengerEntity
            {
                Id = 2,
                Created = created,
                FirtName = "Adam",
                LastName = "Nahlaui",
            });


            builder.Entity<FlightEntity>().HasData(new FlightEntity
            {
                Id = 1,
                Origin = "TLV",
                Destination = "DC",
                Created = created,
                Depart = new DateTime(2021, 08, 22, 22, 00, 00, DateTimeKind.Utc),
                Return = new DateTime(2021, 09, 11, 10, 00, 00, DateTimeKind.Utc),
            });

            builder.Entity<FlightEntity>().HasData(new FlightEntity
            {
                Id = 2,
                Created = created,
                Origin = "NY",
                Destination = "TLV",
                Depart = new DateTime(2021, 08, 23, 09, 00, 00, DateTimeKind.Utc),
                Return = new DateTime(2021, 009, 17, 22, 00, 00, DateTimeKind.Utc),
            });

            builder.Entity<AircraftEntity>().HasData(new AircraftEntity
            {
                Id = 1,
                Created = created,
                FlightRef = 1,
                WeightLimit = 1000,
                BaggagesLimit = 100,
            });

            builder.Entity<AircraftEntity>().HasData(new AircraftEntity
            {
                Id = 2,
                Created = created,
                FlightRef = 2,
                WeightLimit = 100,
                BaggagesLimit = 10,
            });


            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 1,
                Created = created,
                AircraftRef = 1,
                Row = "A",
                Number = 1,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 2,
                Created = created,
                AircraftRef = 1,
                Row = "A",
                Number = 2,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 3,
                Created = created,
                AircraftRef = 1,
                Row = "B",
                Number = 1,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 4,
                Created = created,
                AircraftRef = 1,
                Row = "B",
                Number = 2,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 5,
                Created = created,
                AircraftRef = 2,
                Row = "A",
                Number = 1,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 6,
                Created = created,
                AircraftRef = 2,
                Row = "A",
                Number = 2,
                RowVersion = 1
            });


            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 7,
                Created = created,
                AircraftRef = 2,
                Row = "B",
                Number = 1,
                RowVersion = 1
            });

            builder.Entity<SeatEntity>().HasData(new SeatEntity
            {
                Id = 8,
                Created = created,
                AircraftRef = 2,
                Row = "B",
                Number = 2,
                RowVersion = 1
            });

            base.OnModelCreating(builder);
        }
    }
}



