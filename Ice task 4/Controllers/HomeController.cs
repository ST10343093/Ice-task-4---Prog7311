using System.Diagnostics;
using Ice_task_4.Data;
using Ice_task_4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ice_task_4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Available Rooms
        public async Task<IActionResult> AvailableRooms()
        {
            var availableRooms = await _context.Rooms
                .Where(r => r.IsAvailable)
                .ToListAsync();
            return View(availableRooms);
        }

        // GET: Book Now
        [Authorize]
        public async Task<IActionResult> BookNow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomId == id && m.IsAvailable);

            if (room == null)
            {
                TempData["ErrorMessage"] = "The selected room is not available for booking.";
                return RedirectToAction(nameof(AvailableRooms));
            }

            // Find or create guest record for the current user
            var userEmail = User.Identity.Name;

            // Ensure guest record exists
            await EnsureGuestRecordExists(userEmail);

            // Create booking model with pre-filled data
            var booking = new BookingViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                CheckInDate = DateTime.Now.Date.AddDays(1), // Default check-in: tomorrow
                CheckOutDate = DateTime.Now.Date.AddDays(3), // Default check-out: 2 days stay
                GuestEmail = userEmail
            };

            return View(booking);
        }

        // Helper method to ensure guest record exists
        private async Task<Guest> EnsureGuestRecordExists(string email)
        {
            // Try to find existing guest
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Email == email);

            if (guest == null)
            {
                // Create a new guest with username as first name
                // Extract name parts from email (before @)
                var username = email;
                string firstName = username;
                string lastName = "User";

                // If email format, extract parts
                if (email.Contains("@"))
                {
                    username = email.Split('@')[0];

                    // Attempt to split name parts (e.g., first.last@domain.com)
                    if (username.Contains("."))
                    {
                        var parts = username.Split('.');
                        firstName = parts[0];
                        lastName = string.Join(" ", parts.Skip(1));
                    }
                }

                // Create a new guest record
                guest = new Guest
                {
                    FirstName = char.ToUpper(firstName[0]) + firstName.Substring(1),
                    LastName = char.ToUpper(lastName[0]) + lastName.Substring(1),
                    Email = email,
                    PhoneNumber = ""
                };

                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();

                // Log the creation
                _logger.LogInformation($"Created new guest record for {email}, Guest ID: {guest.GuestId}");
            }

            return guest;
        }

        // POST: Book Now
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> BookNow(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use a transaction to ensure data consistency
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Verify the room is still available
                        var room = await _context.Rooms
                            .FirstOrDefaultAsync(r => r.RoomId == model.RoomId && r.IsAvailable);

                        if (room == null)
                        {
                            TempData["ErrorMessage"] = "Sorry, this room is no longer available.";
                            return RedirectToAction(nameof(AvailableRooms));
                        }

                        // 2. Get or create guest record
                        var userEmail = User.Identity.Name;
                        var guest = await EnsureGuestRecordExists(userEmail);

                        // 3. Calculate stay duration and total price
                        var stayDuration = (model.CheckOutDate - model.CheckInDate).Days;
                        if (stayDuration <= 0)
                        {
                            // Handle invalid dates
                            ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                            return View(model);
                        }

                        var totalPrice = room.PricePerNight * stayDuration;

                        // 4. Create booking with explicit properties
                        var booking = new Booking();
                        booking.RoomId = room.RoomId;
                        booking.GuestId = guest.GuestId;
                        booking.CheckInDate = model.CheckInDate;
                        booking.CheckOutDate = model.CheckOutDate;
                        booking.TotalPrice = totalPrice;
                        booking.IsCancelled = false;

                        // 5. Add booking to context
                        _context.Bookings.Add(booking);
                        await _context.SaveChangesAsync();

                        // 6. Update room availability
                        room.IsAvailable = false;
                        _context.Update(room);
                        await _context.SaveChangesAsync();

                        // 7. Commit the transaction
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = $"Your booking was successful! Room {room.RoomNumber} is reserved for {model.CheckInDate:MMM dd, yyyy} to {model.CheckOutDate:MMM dd, yyyy}.";
                        return RedirectToAction("DiagnoseBookings", "Home");
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if something goes wrong
                        await transaction.RollbackAsync();

                        TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                        return View(model);
                    }
                }
            }

            // If we got here, something failed, redisplay form
            return View(model);
        }

        // Diagnostics Action
        [Authorize]
        public async Task<IActionResult> DiagnoseBookings()
        {
            var userEmail = User.Identity.Name;

            var guests = await _context.Guests.ToListAsync();
            var bookings = await _context.Bookings.ToListAsync();

            string diagnosticInfo = $"Total guests in system: {guests.Count}\n";
            diagnosticInfo += $"Total bookings in system: {bookings.Count}\n\n";

            diagnosticInfo += "All Guests:\n";
            foreach (var guest in guests)
            {
                diagnosticInfo += $"Guest ID: {guest.GuestId}, Name: {guest.FirstName} {guest.LastName}, Email: {guest.Email}\n";
            }

            diagnosticInfo += "\nAll Bookings:\n";
            foreach (var booking in bookings)
            {
                diagnosticInfo += $"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}, Guest ID: {booking.GuestId}, Check-in: {booking.CheckInDate:yyyy-MM-dd}, Check-out: {booking.CheckOutDate:yyyy-MM-dd}\n";
            }

            // Find current user's guest record
            var currentGuest = guests.FirstOrDefault(g => g.Email == userEmail);
            if (currentGuest != null)
            {
                diagnosticInfo += $"\nCurrent User's Guest Record:\n";
                diagnosticInfo += $"Guest ID: {currentGuest.GuestId}, Name: {currentGuest.FirstName} {currentGuest.LastName}, Email: {currentGuest.Email}\n";

                // Find bookings with this guest ID
                var userBookings = bookings.Where(b => b.GuestId == currentGuest.GuestId).ToList();
                diagnosticInfo += $"\nBookings for current user (Count: {userBookings.Count}):\n";
                foreach (var booking in userBookings)
                {
                    diagnosticInfo += $"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}, Check-in: {booking.CheckInDate:yyyy-MM-dd}, Check-out: {booking.CheckOutDate:yyyy-MM-dd}\n";
                }
            }
            else
            {
                diagnosticInfo += $"\nNo guest record found for current user ({userEmail})";
            }

            ViewBag.DiagnosticInfo = diagnosticInfo;
            return View();
        }

        // Test Booking Creation
        [Authorize]
        public async Task<IActionResult> CreateTestBooking()
        {
            try
            {
                // 1. Get a room that's available
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.IsAvailable);
                if (room == null)
                {
                    TempData["ErrorMessage"] = "No available rooms to book.";
                    return RedirectToAction("DiagnoseBookings");
                }

                // 2. Get current user's guest record
                var userEmail = User.Identity.Name;
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guest == null)
                {
                    TempData["ErrorMessage"] = "No guest record found for current user.";
                    return RedirectToAction("DiagnoseBookings");
                }

                // 3. Create a simple booking directly
                var booking = new Booking
                {
                    RoomId = room.RoomId,
                    GuestId = guest.GuestId,
                    CheckInDate = DateTime.Now.AddDays(1),
                    CheckOutDate = DateTime.Now.AddDays(3),
                    TotalPrice = room.PricePerNight * 2,
                    IsCancelled = false
                };

                // 4. Add and save
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // 5. Update room availability
                room.IsAvailable = false;
                _context.Update(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Test booking created! ID: {booking.BookingId}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating test booking: {ex.Message}";
            }

            return RedirectToAction("DiagnoseBookings");
        }
    }
}