using System.ComponentModel.DataAnnotations;

namespace Event_Ease.Models.ViewModels
{   //This model will be userd to bind the data to the Add venue Form
    public class AddVenueViewModel
    {
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }

        public IFormFile? ImageFile { get; set; } // user uploads this

        public string? ImageUrl { get; set; } // Current image for edit display and fallback

        public string Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
