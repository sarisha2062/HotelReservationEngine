using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationEngine.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoomNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } // e.g., "Standard", "Deluxe", "Suite"

        [Required]
        public decimal BasePrice { get; set; }

        public int Capacity { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; }
    }
}