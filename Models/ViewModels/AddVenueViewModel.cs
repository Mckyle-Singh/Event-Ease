namespace Event_Ease.Models.ViewModels
{   //This model will be userd to bind the data to the Add venue Form
    public class AddVenueViewModel
    {
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
