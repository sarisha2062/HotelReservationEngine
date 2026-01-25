
using System;
using System.Collections.Generic;
using System.Linq;
using HotelReservationEngine.Models;
using HotelReservationEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationEngine.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HotelDbContext _context;

        public ReservationService(HotelDbContext context)
        {
            _context = context;
        }

        // Room CRUD operations
        public List<Room> GetAllRooms()
        {
            return _context.Rooms.ToList();
        }

        public Room GetRoomById(int id)
        {
            return _context.Rooms.Find(id);
        }

        public Room CreateRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room;
        }

        public Room UpdateRoom(Room room)
        {
            _context.Rooms.Update(room);
            _context.SaveChanges();
            return room;
        }

        public bool DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return true;
        }

        // Reservation CRUD operations
        public List<Reservation> GetAllReservations()
        {
            return _context.Reservations
                .Include(r => r.Room)
                .Where(r => !r.IsCancelled)
                .ToList();
        }

        public Reservation GetReservationById(int id)
        {
            return _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefault(r => r.Id == id);
        }

        public Reservation CreateReservation(Reservation reservation)
        {
            // Check if room is available for the given dates
            if (!IsRoomAvailable(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate))
            {
                throw new InvalidOperationException("Room is not available for the selected dates.");
            }

            // Calculate price with dynamic pricing
            reservation.TotalPrice = CalculatePrice(reservation.RoomId,
                reservation.CheckInDate, reservation.CheckOutDate);

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return reservation;
        }

        public Reservation UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
            return reservation;
        }

        public bool CancelReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null) return false;

            reservation.IsCancelled = true;
            _context.SaveChanges();
            return true;
        }

        // Business logic methods
        public List<Room> SearchAvailableRooms(DateTime checkIn, DateTime checkOut, int guests = 1)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be before check-out date.");

            // Get all rooms that can accommodate the number of guests
            var allRooms = _context.Rooms
                .Where(r => r.Capacity >= guests && r.IsAvailable)
                .ToList();

            // Filter rooms that are not booked for the given dates
            var availableRooms = new List<Room>();

            foreach (var room in allRooms)
            {
                if (IsRoomAvailable(room.Id, checkIn, checkOut))
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }

        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut)
        {
            // Check for overlapping reservations
            var overlappingReservations = _context.Reservations
                .Where(r => r.RoomId == roomId && !r.IsCancelled)
                .Where(r => (checkIn < r.CheckOutDate && checkOut > r.CheckInDate))
                .ToList();

            return !overlappingReservations.Any();
        }

        public decimal CalculatePrice(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null) return 0;

            decimal totalPrice = 0;
            DateTime currentDate = checkIn.Date;

            while (currentDate < checkOut.Date)
            {
                // Dynamic pricing: higher prices on weekends
                if (currentDate.DayOfWeek == DayOfWeek.Friday ||
                    currentDate.DayOfWeek == DayOfWeek.Saturday ||
                    currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    totalPrice += room.BasePrice * 1.3m; // 20% higher on weekends
                }
                else
                {
                    totalPrice += room.BasePrice;
                }

                currentDate = currentDate.AddDays(1);
            }

            return totalPrice;
        }
    }
}