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

        [HttpGet]
        public async Task<IActionResult>Edit(Guid id)
        {
            var UserEvent = await dbContext.Events.FindAsync(id);
            // Pass the list of venues to the ViewBag
            ViewBag.Venues = dbContext.Venues.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.VenueName
            }).ToList();
            return View(UserEvent);
        }

        public async Task<IActionResult> Edit(Event viewModel)
        {
            var UserEvent = await dbContext.Events.FindAsync(viewModel.EventID);
            if (UserEvent is not null)
            {
                UserEvent.EventName = viewModel.EventName;
                UserEvent.EventStartDate = viewModel.EventStartDate;
                UserEvent.EventEndDate = viewModel.EventEndDate;
                UserEvent.Description = viewModel.Description;
                UserEvent.VenueID = viewModel.VenueID; // Update the venue

                // Save changes to the database
                await dbContext.SaveChangesAsync();

            }
            return RedirectToAction("List", "Events");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var UserEvent = await dbContext.Events.FindAsync(id);
            if (UserEvent != null)
            {
                dbContext.Events.Remove(UserEvent);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Events");
        }

    }
}
