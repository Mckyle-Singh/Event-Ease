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
                .Select(v => new SelectListItem
                {
                    Value = v.VenueID.ToString(),
                    Text = v.VenueName
                }).ToList()
             };


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventViewModel viewModel)
        {
            // Validate: End date must not be before start date
            if (viewModel.EventEndDate < viewModel.EventStartDate)
            {
                ModelState.AddModelError("", "End date cannot be earlier than the start date.");

                // Reload dropdown list
                viewModel.Venues = await dbContext.Venues
                    .Select(v => new SelectListItem
                    {
                        Value = v.VenueID.ToString(),
                        Text = v.VenueName
                    }).ToListAsync();

                return View(viewModel);
            }

            // Validate: Check for booking conflict at the venue

            if (viewModel.VenueID.HasValue)
            {
                bool isVenueBooked = await dbContext.Events
                    .AnyAsync(e =>
                        e.VenueID == viewModel.VenueID &&
                        e.EventStartDate <= viewModel.EventEndDate &&
                        e.EventEndDate >= viewModel.EventStartDate);

                if (isVenueBooked)
                {
                    ModelState.AddModelError("", "This venue already has a scheduled event for the selected dates.");

                    // Reload dropdown list
                    viewModel.Venues = await dbContext.Venues
                        .Select(v => new SelectListItem
                        {
                            Value = v.VenueID.ToString(),
                            Text = v.VenueName
                        }).ToListAsync();

                    return View(viewModel);
                }
            }

            // Save the event if validation passes
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

            TempData["SuccessMessage"] = "Event successfully created.";

            return RedirectToAction("List", "Events");

        }

        // Show the event list with filtering
        [HttpGet]
        public async Task<IActionResult> List(string searchQuery, Guid? venueId, DateTime? startDate, DateTime? endDate)
        {
            var eventsQuery = dbContext.Events.Include(e => e.Venue).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchQuery));
            }

            if (venueId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.VenueID == venueId.Value);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventStartDate >= startDate.Value && e.EventEndDate <= endDate.Value);
            }

            var filteredEvents = await eventsQuery.ToListAsync();

            // 🔹 Populate venue list for filtering dropdown
            ViewBag.Venues = dbContext.Venues.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.VenueName
            }).ToList();
            return View(filteredEvents);
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
            if (UserEvent is null)
            {
                return RedirectToAction("List", "Events");
            }

            // End date must not be before start date
            if (viewModel.EventEndDate < viewModel.EventStartDate)
            {
                ModelState.AddModelError("", "End date cannot be earlier than the start date.");
            }

            // Check for overlapping events at the same venue (excluding current event)
            if (viewModel.VenueID.HasValue)
            {
                bool isVenueBooked = await dbContext.Events
                    .AnyAsync(e =>
                        e.EventID != viewModel.EventID && // Exclude this event
                        e.VenueID == viewModel.VenueID &&
                        e.EventStartDate <= viewModel.EventEndDate &&
                        e.EventEndDate >= viewModel.EventStartDate);

                if (isVenueBooked)
                {
                    ModelState.AddModelError("", "This venue already has a scheduled event for the selected dates.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Reload venues for the dropdown
                ViewBag.Venues = await dbContext.Venues
                    .Select(v => new SelectListItem
                    {
                        Value = v.VenueID.ToString(),
                        Text = v.VenueName
                    }).ToListAsync();

                return View(viewModel); // Re-display form with validation errors
            }

            // If validation passes, update the event
            UserEvent.EventName = viewModel.EventName;
            UserEvent.EventStartDate = viewModel.EventStartDate;
            UserEvent.EventEndDate = viewModel.EventEndDate;
            UserEvent.Description = viewModel.Description;
            UserEvent.VenueID = viewModel.VenueID;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Events");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Find the event and include related bookings
            var eventItem = await dbContext.Events
                .Include(e => e.Bookings) // Include bookings for validation
                .FirstOrDefaultAsync(e => e.EventID == id);

            // Check if the event exists
            if (eventItem == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("List", "Events");
            }

            // Check if the event has active bookings
            if (eventItem.Bookings.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete an event linked to active bookings.";
                return RedirectToAction("List", "Events");
            }

            // Proceed with deletion if no bookings are linked
            dbContext.Events.Remove(eventItem);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event successfully deleted.";
            return RedirectToAction("List", "Events");
        }

    }
}
