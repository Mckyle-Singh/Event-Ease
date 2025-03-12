namespace Event_Ease.Models.Entities
{
    public class Event
    {
        public Guid EventID { get; set; } // Primary key
        public string EventName { get; set; } // Event name
        public DateTime EventDate { get; set; } // Event date
        public string Description { get; set; }

        // Nullable foreign key for Venue
        // Navigation property (optional)
        public Guid? VenueID { get; set; }
        public Venue? Venue { get; set; } 
    }
}
