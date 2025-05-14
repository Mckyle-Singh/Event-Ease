using Event_Ease.Data;
using Event_Ease.Models.Entities;
using Event_Ease.Models.ViewModels;
using Event_Ease.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Event_Ease.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlobStorageService _blobService;
        private readonly string _containerName;

        public VenuesController(ApplicationDbContext dbContext, IBlobStorageService blobService, IConfiguration configuration)
        {
          
            this.dbContext = dbContext;
            _blobService = blobService;
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddVenueViewModel viewModel)
        {
         
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Validation errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return View(viewModel);
            }

            string imageUrl = null;

            if (viewModel.ImageFile?.Length > 0)
            {
                // Upload image to Azure Blob Storage
                Console.WriteLine($"ImageFile is null? {viewModel.ImageFile == null}");
                imageUrl = await _blobService.UploadFileAsync(viewModel.ImageFile, _containerName);
            }

            // Fallback image if none was uploaded
            imageUrl ??= "https://picsum.photos/200/300";
            // Log the ImageUrl to the console to verify its value
            Console.WriteLine($"Image URL: {imageUrl}"); // Console log for debugging
            Debug.WriteLine($"Image URL: {imageUrl}"); // If you use Visual Studio, this will appear in the Output window

            var venue = new Venue
            {
                VenueID = Guid.NewGuid(),
                VenueName = viewModel.VenueName,
                Location = viewModel.Location,
                Capacity = viewModel.Capacity,
                ImageUrl = imageUrl,
                Description = viewModel.Description,
                IsActive = viewModel.IsActive
            };

            dbContext.Venues.Add(venue);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Venues");
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchQuery, string location, int? minCapacity, int? maxCapacity)
        {
            var venuesQuery = dbContext.Venues.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                venuesQuery = venuesQuery.Where(v => v.VenueName.Contains(searchQuery));
            }

            if (minCapacity.HasValue)
            {
                switch (minCapacity.Value)
                {
                    case 1:
                        venuesQuery = venuesQuery.Where(v => v.Capacity >= 1 && v.Capacity <= 50);
                        break;
                    case 51:
                        venuesQuery = venuesQuery.Where(v => v.Capacity >= 51 && v.Capacity <= 100);
                        break;
                    case 101:
                        venuesQuery = venuesQuery.Where(v => v.Capacity >= 101 && v.Capacity <= 200);
                        break;
                    case 200:
                        venuesQuery = venuesQuery.Where(v => v.Capacity >= 200); // "200+" case
                        break;
                }
            }
            // Filter by exact location (from dropdown)
            if (!string.IsNullOrWhiteSpace(location))
            {
                venuesQuery = venuesQuery.Where(v => v.Location == location);
            }
            
            var venues = await venuesQuery.ToListAsync();
           
            return View(venues);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
           var venue = await dbContext.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();

            var viewModel = new AddVenueViewModel
            {
                VenueID = venue.VenueID,
                VenueName = venue.VenueName,
                Location = venue.Location,
                Capacity = venue.Capacity,
                ImageUrl = venue.ImageUrl, // Show existing image
                Description = venue.Description,
                IsActive = venue.IsActive
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddVenueViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                // Log all validation errors to the console
                Console.WriteLine("ModelState is invalid. Validation errors:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"- {state.Key}: {error.ErrorMessage}");
                    }
                }

                return View(viewModel);
            }

            var venue = await dbContext.Venues.FindAsync(viewModel.VenueID);
            if (venue == null)
            {
                Console.WriteLine("Venue not found.");
                return NotFound();
            }

            // Upload new image if one is provided
            if (viewModel.ImageFile?.Length > 0)
            {
                try
                {
                    var imageUrl = await _blobService.UploadFileAsync(viewModel.ImageFile, _containerName);
                    venue.ImageUrl = imageUrl;
                    Console.WriteLine($"Uploaded new image: {imageUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Image upload failed: {ex.Message}");
                    ModelState.AddModelError("ImageFile", "Image upload failed.");
                    return View(viewModel);
                }
            }
            else
            {
                // No new image uploaded — preserve existing one
                venue.ImageUrl = viewModel.ImageUrl;
                Console.WriteLine("No new image uploaded. Keeping existing image.");
            }

            // Update other fields
            venue.VenueName = viewModel.VenueName;
            venue.Location = viewModel.Location;
            venue.Capacity = viewModel.Capacity;
            venue.Description = viewModel.Description;
            venue.IsActive = viewModel.IsActive;

            await dbContext.SaveChangesAsync();
            Console.WriteLine("Venue updated successfully.");

            return RedirectToAction("List", "Venues");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var venue = await dbContext.Venues
        .Include(v => v.Bookings) // Ensure Bookings are included in the query
        .FirstOrDefaultAsync(v => v.VenueID == id);

            // Check if venue is null
            if (venue == null)
            {
                TempData["ErrorMessage"] = "Venue not found.";
                return RedirectToAction("List", "Venues"); // Redirect back to the list view
            }

            // Check if venue has active bookings
            if (venue.Bookings.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete a venue linked to active bookings.";
                return RedirectToAction("List", "Venues"); // Redirect back to the list view
            }

            // Proceed with deletion
            dbContext.Venues.Remove(venue);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Venue successfully deleted.";
            return RedirectToAction("List", "Venues"); // Redirect back to the list view
        }

    }
}
