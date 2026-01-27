using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelReservationEngine.Data;
using HotelReservationEngine.Services;
using HotelReservationEngine.Models;

namespace HotelReservationEngine
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IReservationService _reservationService;
        

        static void Main(string[] args)
        {
            // Setup Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            //InitializeDatabase
            InitializeDatabase();

            _reservationService = _serviceProvider.GetService<IReservationService>();

            Console.WriteLine("=== Hotel Reservation Engine ===");
            ShowMainMenu();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(@"Server=localhost;Database=HotelDb;Trusted_Connection=True;TrustServerCertificate=True;"));

            services.AddScoped<IReservationService, ReservationService>();
        }

        static void InitializeDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Console.WriteLine("Database initialized!");
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n====================MAIN MENU=============================");
                Console.WriteLine("1. Manage Rooms");
                Console.WriteLine("2. Manage Reservations");
                Console.WriteLine("3. Search Available Rooms");
                Console.WriteLine("4. Exit");
                Console.WriteLine("Select Option:");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowRoomMenu();
                        break;
                    case "2":
                        ShowReservationMenu();
                        break;
                    case "3":
                        SearchAvailableRooms();
                        break;
                    case "4":
                        Console.WriteLine("Thank you for using our services");
                        return;
                    default:
                        Console.WriteLine("Please choose valid option!");
                        break;
                }
            }
        }

        static void ShowRoomMenu()
        {
            while (true)
            {
                Console.WriteLine("\n================ROOM MANAGEMENT=====================");
                Console.WriteLine("1. View All Rooms.");
                Console.WriteLine("2. Add New Room");
                Console.WriteLine("3. Update Room");
                Console.WriteLine("4. Delete Room");
                Console.WriteLine("5. Back to Main Menu");
                Console.WriteLine("Select Option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllRooms();
                        break;
                    case "2":
                        AddNewRoom();
                        break;
                    case "3":
                        UpdateRoom();
                        break;
                    case "4":
                        DeleteRoom();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Please Choose Valid Option!");
                        break;
                }
            }
        }

        static void ShowReservationMenu()
        {
            while (true)
            {
                Console.WriteLine("\n================RESERVATION MANAGEMENT=====================");
                Console.WriteLine("1. View All Reservations.");
                Console.WriteLine("2. Create Reservation");
                Console.WriteLine("3. Cancel Reservation");
                Console.WriteLine("4. View Reservation Details");
                Console.WriteLine("5. Back to Main Menu");
                Console.WriteLine("Select Option: ");


                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllReservations();
                        break;
                    case "2":
                        CreateReservation();
                        break;
                    case "3":
                        CancelReservation();
                        break;
                    case "4":
                        ViewReservationDetails();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Please Choose Valid Option!");
                        break;
                }
            }
        }

        static void ViewAllRooms()
        {
            var rooms = _reservationService.GetAllRooms();
            Console.WriteLine("\n==============ALL ROOMS=====================");
            foreach (var room in rooms)
            {
                Console.WriteLine($"ID: {room.Id}, Room: {room.RoomNumber}, Type: {room.Type}, Description: {room.description}, Price: ${room.BasePrice}, Capacity: {room.capacity}, Available: {room.IsAvailable}");
            }
        }

        static void AddNewRoom()
        {
            Console.WriteLine("\n==============ADD NEW ROOM=====================");

            Console.WriteLine("Room Number: ");
            var roomNumber = Console.ReadLine();

            Console.WriteLine("Room Type: ");
            var type = Console.ReadLine();

            Console.WriteLine("Base Price: ");
            var basePrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Capacity: ");
            var capacity = int.Parse(Console.ReadLine());


            Console.WriteLine("Description: ");
            var description = Console.ReadLine();

            var room = new Room
            {
                RoomNumber = roomNumber,
                Type = type,
                BasePrice = basePrice,
                capacity = capacity,
                description = description,
                IsAvailable = true
            };

            _reservationService.CreateRoom(room);
            Console.WriteLine("Room added successfully!");
        }

        static void UpdateRoom()
        {
            Console.Write("Enter Room ID to Update: ");
            var id = int.Parse(Console.ReadLine());


            var room = _reservationService.GetRoomById(id);
            if (room == null)
            {
                Console.WriteLine("Room not found");
                return;
            }


            Console.Write($"Room Number ({room.RoomNumber}): ");
            var roomNumber = Console.ReadLine();
            if (!string.IsNullOrEmpty(roomNumber)) room.RoomNumber = roomNumber;

            Console.Write($"Room Type ({room.Type}): ");
            var type = Console.ReadLine();
            if (!string.IsNullOrEmpty(type)) room.Type = type;

            Console.Write($"Base Price ({room.BasePrice}): ");
            var priceInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(priceInput)) room.BasePrice = decimal.Parse(priceInput);

            _reservationService.UpdateRoom(room);
            Console.WriteLine("Room update successfully!");
        }

        static void DeleteRoom()
        {
            Console.Write("Enter Room ID to delete: ");
            var id = int.Parse(Console.ReadLine());

            if (_reservationService.DeleteRoom(id))
                Console.WriteLine("Room deleted Successfully!");
            else
                Console.WriteLine("Room not found!");
        }

        static void ViewAllReservations()
        {
            var reservations = _reservationService.GetAllReservations();
            Console.WriteLine("\n================ALL RESERVATIONS===================");
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"ID: {reservation.Id}, Guest: {reservation.GuestName}, Room: {reservation.Room?.RoomNumber}, Check-in: {reservation.CheckInDate:yyyy-MM-dd}, Check-out: {reservation.CheckOutDate:yyyy-MM-dd}, Price: ${reservation.TotalPrice}");
            }
        }

        static void CreateReservation()
        {
            Console.WriteLine("\n=== CREATE RESERVATION ===");

            // First, show available rooms for selected dates
            Console.Write("Check-in Date (yyyy-MM-dd): ");
            var checkIn = DateTime.Parse(Console.ReadLine());

            Console.Write("Check-out Date (yyyy-MM-dd): ");
            var checkOut = DateTime.Parse(Console.ReadLine());

            Console.Write("Number of Guests: ");
            var guests = int.Parse(Console.ReadLine());

            try
            {
                var availableRooms = _reservationService.SearchAvailableRooms(checkIn, checkOut, guests);

                if (!availableRooms.Any())
                {
                    Console.WriteLine("No rooms available for the selected dates!");
                    return;
                }

                Console.WriteLine("\n=== AVAILABLE ROOMS ===");
                foreach (var room in availableRooms)
                {
                    var price = _reservationService.CalculatePrice(room.Id, checkIn, checkOut);
                    Console.WriteLine($"ID: {room.Id}, Room: {room.RoomNumber}, Type: {room.Type}, Price: ${price}, Capacity: {room.capacity}");
                }

                Console.Write("Select Room ID: ");
                var roomId = int.Parse(Console.ReadLine());

                Console.Write("Guest Name: ");
                var guestName = Console.ReadLine();

                Console.Write("Guest Email: ");
                var guestEmail = Console.ReadLine();

                Console.Write("Guest Phone: ");
                var guestPhone = Console.ReadLine();

                Console.Write("Special Requests: ");
                var specialRequests = Console.ReadLine();

                var reservation = new Reservation
                {
                    RoomId = roomId,
                    GuestName = guestName,
                    GuestEmail = guestEmail,
                    GuestPhone = guestPhone,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    NumberOfGuests = guests,
                    SpecialRequests = specialRequests
                };

                var createdReservation = _reservationService.CreateReservation(reservation);
                Console.WriteLine($"Reservation created successfully! Total Price: ${createdReservation.TotalPrice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void CancelReservation()
        {

            Console.Write("Enter Reservation IT to Cancel: ");
            var id = int.Parse(Console.ReadLine());
            if (_reservationService.CancelReservation(id))
                Console.WriteLine("Reservation Cancelled successfully!");
            else
                Console.WriteLine("Reservation Not found!");
        }

        static void ViewReservationDetails()
        {

            Console.Write("Enter Reservation Id: ");
            var id = int.Parse(Console.ReadLine());

            var reservation = _reservationService.GetReservationById(id);

            if (reservation == null)
            {
                Console.WriteLine("Reservation Not Found!");
                return;
            }
            Console.WriteLine("\n========Reservation Details=========");
            Console.WriteLine($"ID: {reservation.Id}");
            Console.WriteLine($"Guest: {reservation.GuestName}");
            Console.WriteLine($"Email: {reservation.GuestEmail}");
            Console.WriteLine($"Phone: {reservation.GuestPhone}");
            Console.WriteLine($"Room: {reservation.Room?.RoomNumber}({reservation.Room?.Type})");
            Console.WriteLine($"Check-in: {reservation.CheckInDate:yyyy-MM-dd}");
            Console.WriteLine($"Check-out: {reservation.CheckOutDate:yyyy-MM-dd}");
            Console.WriteLine($"Guests: {reservation.NumberOfGuests}");
            Console.WriteLine($"Total Price: {reservation.TotalPrice}");
            Console.WriteLine($"Special Request: {reservation.SpecialRequests}");
            Console.WriteLine($"Status: {(reservation.IsCancelled ? "Cancelled" : "Active")}");
        }

        static void SearchAvailableRooms()
        {

            Console.WriteLine("\n============Search Available Rooms================");

            Console.WriteLine("Check-in Date(yyyy-MM-dd): ");
            var checkIn = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Check-out Date(yyyy-MM-dd): ");
            var checkOut = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Number of Guests(default 1): ");
            var guestsInput = Console.ReadLine();
            var guests = string.IsNullOrEmpty(guestsInput) ? 1 : int.Parse(guestsInput);


            try
            {
                var availableRooms = _reservationService.SearchAvailableRooms(checkIn, checkOut, guests);

                if (!availableRooms.Any())
                {
                    Console.WriteLine("No rooms available for the selected dates!");
                    return;
                }

                Console.WriteLine("\n==========Available Rooms=============");
                foreach (var room in availableRooms)
                {
                    var price = _reservationService.CalculatePrice(room.Id, checkIn, checkOut);
                    Console.WriteLine($"ID: {room.Id}, Room: {room.RoomNumber}, Type: {room.Type}, Base Price: ${room.BasePrice}/night, Total Price: ${price}, Capacity: {room.capacity} ");
                    Console.WriteLine($"Description: {room.description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
