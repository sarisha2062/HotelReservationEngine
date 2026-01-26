using System.Collections.Generic;
using System.ComponentModel.DataAnnotatiions;


namespace HotelReservationEngine.Models
{
    public class Room
    {
        [Key]
        public int Id{get; set;}
        
        [Required]
        [MaxLength(50)]
        public string RoomNumber{get; set;}

        [Required]
        [MaxLength(100)]
        public string Type{get; set;} //example :"standard", "deluxe", "suite"

        public int capacity{get; set;}

        [Required]
        public decimal BasePrice{get; set;}
        
        [MaxLength(500)]
        public string description{get; set;}

        public bool IsAvailable{get; set;} = true;

        public ICollection<Reservation> Reservations { get; set; }
    }
}
