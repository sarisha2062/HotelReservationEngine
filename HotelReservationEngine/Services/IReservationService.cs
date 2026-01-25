using System;
using System.Collections.Generic;
using HotelReservationEngine.Models;

namespace HotelReservationEngine.Services
{
    public interface IReservationService
    {
        // Room operations
        List<Room> GetAllRooms();
        Room GetRoomById(int id);
        Room CreateRoom(Room room);
        Room UpdateRoom(Room room);
        bool DeleteRoom(int id);

        // Reservation operations
        List<Reservation> GetAllReservations();
        Reservation GetReservationById(int id);
        Reservation CreateReservation(Reservation reservation);
        Reservation UpdateReservation(Reservation reservation);
        bool CancelReservation(int id);

        // Business logic
        List<Room> SearchAvailableRooms(DateTime checkIn, DateTime checkOut, int guests = 1);
        bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut);
        decimal CalculatePrice(int roomId, DateTime checkIn, DateTime checkOut);
    }
}