using Event_Ease.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Ease.Models.ViewModels
{
    public class AddEventViewModel
    {
        public string EventName { get; set; } // Event name
        public DateTime EventStartDate { get; set; } // Event date
        public DateTime EventEndDate { get; set; } // Event date
        public string Description { get; set; }//Description of Event

        // Nullable foreign key for Venue
        // Navigation property (optional)
        public Guid? VenueID { get; set; }
        public List<SelectListItem> Venues { get; set; } // List of venues for dropdown options

    }
}
