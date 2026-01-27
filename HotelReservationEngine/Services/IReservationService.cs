using System;
using System.Collections.Generic;
using System.Linq;
using HotelReservationEngine.Models;
using HotelReservationEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationEngine.Services
{
    public interface IReservationService
    {
        // Operations for managing room reservations
        List<Room> GetAllRooms ();
        Room GetRoomById (int id );
        Room CreateRoom ( Room room );
        Room UpdateRoom ( Room room );
        bool DeleteRoom ( int id );

        // Operations for managing reservations
        List<Reservation> GetAllReservations ();
        Reservation GetReservationById ( int id );
        Reservation CreateReservation ( Reservation reservation );
        Reservation UpdateReservation ( Reservation reservation );
        bool CancelReservation ( int id );

        // Additional operations for reservation
        List<Room> SearchAvailableRooms( DateTime checkIn, DateTime checkOut, int guests = 1 );
        bool IsRoomAvailable( int roomId, DateTime checkIn, DateTime checkOut );
        decimal CalculatePrice ( int roomId, DateTime checkIn, DateTime checkOut );
    }
}