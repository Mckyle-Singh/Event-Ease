using Event_Ease.Data;
using Event_Ease.Models.Entities;
using Event_Ease.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Event_Ease.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public VenuesController(ApplicationDbContext dbContext)
        {
          
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddVenueViewModel viewModel)
        {
            var venue = new Venue
            {
                VenueName = viewModel.VenueName,
                Capacity = viewModel.Capacity,
                Location = viewModel.Location,
                ImageUrl = viewModel.ImageUrl,
                Description = viewModel.Description,
                IsActive = viewModel.IsActive,
            };

            await dbContext.Venues.AddAsync(venue);
            await dbContext.SaveChangesAsync();

            return View();
        }
    }
}
