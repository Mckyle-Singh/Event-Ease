namespace Event_Ease.Models.Entities
{
    public class Venue
    {
        public Guid VenueID { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }

        // Navigation property for related events
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
