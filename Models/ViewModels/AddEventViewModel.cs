using Event_Ease.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Event_Ease.Models.ViewModels
{
    public class AddEventViewModel
    {
        [Display(Name = "Event Name")]
        public string EventName { get; set; } // Event name

        [Display(Name = "Event Start Date")]
        public DateTime EventStartDate { get; set; } // Event date

        [Display(Name = "Event End Date")]
        public DateTime EventEndDate { get; set; } // Event date

        public string Description { get; set; }//Description of Event

        // Nullable foreign key for Venue
        // Navigation property (optional)
        public Guid? VenueID { get; set; }
        public List<SelectListItem> Venues { get; set; } // List of venues for dropdown options

    }
}
