namespace Event_Ease.Models.Entities
{
    public class Booking
    {
        public Guid BookingID { get; set; }
        public Guid EventID { get; set; }
        public Guid VenueID { get; set; }
        public DateTime BookingDate { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Venue Venue { get; set; }
    }
}
