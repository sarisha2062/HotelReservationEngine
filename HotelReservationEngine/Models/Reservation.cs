using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HotelReservationEngine.Models
{
    [key]
    public int Id{get; set;}

    [Required]
    [ForeignKey("Room")]
    public int RoomId{get; set;}
    public Room Room{get; set;}

    [Required]
    [MaxLength(100)]
    public string GuestName{get; set;}

    [Required]
    [Phone]
    public string GuestPhone{get; set;}

    [Required]
    [EmailAddress]
    public string GuestEmail{get; set;}

    [Required]
    public DateTime CheckInDate{get; set;}

    [Required]
    public DateTime CheckOutDate{get; set;}

    [Required]
    public int NumberOfGuests{get; set;}

    public decimal TotalPrice{get; set;}

    public DateTime ReservationDate{get; set;} = DateTime.Now;

    [MaxLength(500)]
    public string SpecialRequests{get; set;}

    public bool IsCancelled{get; set;} = false;
}
