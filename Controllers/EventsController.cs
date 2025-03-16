using Event_Ease.Data;
using Event_Ease.Models.Entities;
using Event_Ease.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Ease.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EventsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var viewModel = new AddEventViewModel
            {
                Venues = dbContext.Venues
            .Where(v => !dbContext.Events.Any(e =>
                e.VenueID == v.VenueID &&
                (e.EventStartDate <= DateTime.Today && e.EventEndDate >= DateTime.Today))) // Replace with custom logic if needed
            .Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.VenueName
            })
            .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventViewModel viewModel)
        {
            var Userevent = new Event
            {
                EventName = viewModel.EventName,
                EventStartDate = viewModel.EventStartDate,
                EventEndDate = viewModel.EventEndDate,
                Description = viewModel.Description,
                VenueID = viewModel.VenueID, 

            };
            await dbContext.Events.AddAsync(Userevent);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Venues");

        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var UserEvents = await dbContext.Events.Include(e=>e.Venue).ToListAsync();
            return View(UserEvents);
        }
    }
}
