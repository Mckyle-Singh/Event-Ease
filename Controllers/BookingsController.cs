using Event_Ease.Data;
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
    }
}
