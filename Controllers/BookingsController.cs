using Event_Ease.Data;
using Event_Ease.Models.Entities;
using Event_Ease.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Event_Ease.Models.ViewModels.AddBookingViewModel;

namespace Event_Ease.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public BookingsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
             var events = dbContext.Events
            .Include(e => e.Venue)
            .Where(e => e.Venue != null)
            .Select(e => new SelectListItem
            {
                Value = e.EventID.ToString(),
                Text = e.EventName
            }).ToList();

                var viewModel = new AddBookingViewModel
                {
                    BookingDate = DateTime.Today, // Optional default value
                    Events = events // Populate events dropdown
                };
             return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookingViewModel viewModel)
        {
            // Fetch the event to retrieve the linked VenueID
            var eventEntity = await dbContext.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventID == viewModel.EventID);

            if (eventEntity == null || eventEntity.Venue == null)
            {
                // Handle the case where the event or venue is invalid
                ModelState.AddModelError("", "Invalid event or venue selection.");

                // Reload events dropdown to re-display the form properly
                viewModel.Events = await dbContext.Events
                    .Include(e => e.Venue)
                    .Where(e => e.Venue != null)
                    .Select(e => new SelectListItem
                    {
                        Value = e.EventID.ToString(),
                        Text = e.EventName
                    }).ToListAsync();

                return View(viewModel);
            }

            // Map data to Booking entity
            var booking = new Booking
            {
                BookingDate = viewModel.BookingDate,
                EventID = viewModel.EventID,
                VenueID = eventEntity.Venue.VenueID // Assign VenueID from the event
            };

            await dbContext.Bookings.AddAsync(booking);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Events");
        }

        [HttpGet]

        public async Task<IActionResult> List()
        {
            var bookings = await dbContext.Bookings.Include(e=> e.Event).ThenInclude(v=>v.Venue).ToListAsync();
            return View(bookings);
        }
        
    }
}
