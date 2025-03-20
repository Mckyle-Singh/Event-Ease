using Event_Ease.Data;
using Event_Ease.Models.Entities;
using Event_Ease.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            return RedirectToAction("List", "Venues");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
          var venues =  await dbContext.Venues.ToListAsync();
           
          return View(venues);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
           var venue = await dbContext.Venues.FindAsync(id);

            return View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Venue viewModel)
        {
           var venue= await dbContext.Venues.FindAsync(viewModel.VenueID);

            if(venue is not null)
            {
                venue.VenueName = viewModel.VenueName;
                venue.Location = viewModel.Location;
                venue.ImageUrl = viewModel.ImageUrl;
                venue.Description = viewModel.Description;
                venue.IsActive = viewModel.IsActive;
                venue.Capacity = viewModel.Capacity;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List","Venues");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var venue = await dbContext.Venues.FindAsync(id);
            if (venue is not null)
            { 
              dbContext.Remove(venue);
              await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Venues");
        }

    }
}
