using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Ease.Models.ViewModels
{
    public class AddBookingViewModel
    {
        
            public Guid BookingID { get; set; } // Optional, if required for editing/updating bookings
            public Guid EventID { get; set; } // Selected event
            public Guid VenueID { get; set; } // Selected venue
            public DateTime BookingDate { get; set; } // Date when booking was made

            // Dropdown lists
            public List<SelectListItem> Events { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> Venues { get; set; } = new List<SelectListItem>();


        
    }
}
