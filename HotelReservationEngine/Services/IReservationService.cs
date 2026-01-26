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

        public ReservationService( HotelDbContext context )
        {
            _context = context;
        }

        // Implementations of IReservationService methods go here
        // For brevity, only a couple of methods are implemented as examples

        public List<Room> GetAllRooms()
        {
            return _context.Rooms.ToList();
        }

        public Room GetRoomById( int id )
        {
            return _context.Rooms.Find( id );
        }

        public Room CreateRoom(Room room)
        {
            _context.Rooms.Add( room );
            _context.SaveChanges();
            return room;
        }

        public Room UpdateRoom( Room room )
        {
            _context.Rooms.Update( room );
            _context.SaveChanges();
            return room;
        }

        public bool DeleteRoom( int id )
        {
            var room = _context.Rooms.Find( id );
            if ( room == null ) return false;

            _context.Rooms.Remove( room );
            _context.SaveChanges();
            return true;
        }

        // Additional methods would be implemented similarly
        
        public List<Reservation> GetAllReservations()
        {
            return _context.Reservations
            .Include( r => r.Room )
            .Where( r => !r.IsCancelled )
            .ToList();
        }

        public Reservation GetReservationById( int id )
        {
            return _context.Reservations
            .Include( r => r.Room )
            .FirstOrDefault( r => r.Id == id && !r.IsCancelled );
        }

        public Reservation CreateReservation( Reservation reservation )
        {
            // To check room availability before creating a reservation
            if ( !IsRoomAvailable( reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate ) )
            {
                throw new InvalidOperationException( "Room is not available for the selected dates." );
            }
        }

        // Calculate total price based on room rate and duration
        reservation.TotalPrice = CalculatePrice( reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate );
        _context.Reservations.Add( reservation );
        _context.SaveChanges();

        return reservation;

        public Reserc=vation UpdateReservation( Reservation reservation )
        {
            _context.Reservations.Update( reservation );
            _context.SaveChanges();
            return reservation;
        }

        public boll CancelReservation( int id )
        {
            var reservation = _context.Reservations.Find( id );
            if ( reservation == null || reservation.IsCancelled ) 
            {
                return false;
            }

            reservation.IsCancelled = true;
            _context.SaveChanges();
            return true;
        }

        public List<Room> SearchAvailableRooms( DateTime checkIn, DateTime checkOut, int guests = 1 )
        {
            // Validate dates for search
            if ( checkIn >= checkOut )
            {
                throw new ArgumentException( "Check-out date must be after check-in date Or Enter valid dates." );
            }

            // Find rooms that can be assigned to the guests.
            var allRooms = _context.Rooms
            .Where( r => r.Capacity >= guests && r.IsAvailable )
            .ToList();

            // Filter rooms that are not booked for the gven dates.
            var availableRooms = new List<Room> ();
            foreach ( var room in allRooms )
            {
                if (IsRoomAvailable ( room.Id, checkIn, checkOut ))
                {
                    availableRooms.Add( room );
                }
            }
            return availableRooms;
        }

        public bool IsRoomAvailable ( int roomId, DateTime checkIn, DateTime checkOut )
        {
            // Prevents for overlapping reservation or double reservation.
            var overlappingReservations = _context.Reservations
            .Where ( r => r.RoomId == roomId && !r.IsCancelled )
            .Where ( r => checkIn < r.CheckOutDate && checkOut > r.CheckInDate )
            .ToList();

            return !overlappingReservations.Any();
        }

        public decimal CalculatePrice ( int roomId, DateTime checkIn, DateTime checkOut )
        {
            var room = _context.Rooms.Find( roomId );
            if ( room == null )
            return 0;

            decimal totalPrice = 0;
            DateTime currentDate = checkIn.Date;

            while ( currentDate < checkOut.Date )
            {
                if ( currentDate.DayOfWeek == DayOfWeek.Friday || currentDate.DayOfWeek == DayOfWeek.Saturday )
                {
                    totalPrice += room.BasePrice * 1.5m; // Apply 50% surcharge on weekends
                }
                else
                {
                    totalPrice += room.BasePrice;
                }
                currentDate = currentDate.AddDays( 1 );
            }
            return totalPrice;
        }
    }
}
