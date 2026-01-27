using Microsoft.EntityFrameworkCore;
using HotelReservationEngine.Models;

namespace HotelReservationEngine.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }
        public DbSet <Room> Rooms{get; set;}
        public DbSet <Reservation> Reservations{get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Confirure relationships and constrains
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            //Seed initial data
            modelBuilder.Entity<Room>().HasData (
                new Room
                {
                    Id = 1,
                    RoomNumber = "101",
                    Type = "Standard",
                    BasePrice = 100,
                    capacity = 2,
                    description = "Standard room with queen bed" 
                },
                new Room
                {
                    Id = 2,
                    RoomNumber = "102",
                    Type = "Standard",
                    BasePrice = 200,
                    capacity = 2,
                    description = "Standard room with twin bed"
                },
                new Room
                {
                    Id = 3,
                    RoomNumber = "103",
                    Type = "Standard",
                    BasePrice = 300,
                    capacity = 2,
                    description = "Standard room with queen bed"
                },
                new Room
                {
                    Id = 4,
                    RoomNumber = "201",
                    Type = "Delux",
                    BasePrice = 400,
                    capacity = 2,
                    description = "Delux room with king bed"
                },
                new Room
                {
                    Id = 5,
                    RoomNumber = "202",
                    Type = "Delux",
                    BasePrice = 500,
                    capacity = 2,
                    description = "Delux room with king bed"
                },
                new Room
                {
                    Id = 6,
                    RoomNumber = "203",
                    Type = "Delux",
                    BasePrice = 600,
                    capacity = 2,
                    description = "Delux room with king bed"
                },
                new Room
                {
                    Id = 7,
                    RoomNumber = "301",
                    Type = "Luxurious",
                    BasePrice = 700,
                    capacity = 5,
                    description = "Luxurious room with king bed and 2 normal bed"
                },
                new Room
                {
                    Id = 8,
                    RoomNumber = "302",
                    Type = "Luxurious",
                    BasePrice = 800,
                    capacity = 4,
                    description = "Luxurious room with king bed and living area"
                },
                new Room
                {
                    Id = 9,
                    RoomNumber = "303",
                    Type = "Luxurious",
                    BasePrice = 900,
                    capacity = 3,
                    description = "Luxurious room with king bed"
                }
            );
        }
    }
}