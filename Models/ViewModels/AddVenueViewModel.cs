using System.ComponentModel.DataAnnotations;

namespace Event_Ease.Models.ViewModels
{   //This model will be userd to bind the data to the Add venue Form
    public class AddVenueViewModel
    {
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [Display(Name = "Venue Image URL")]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
