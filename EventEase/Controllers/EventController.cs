using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()

        {
            var events = await _context.Events
                .Include(i => i.Venue)
                .ToListAsync();

            return View(events);
        }
        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event even)
        {
            if (ModelState.IsValid)
            {

                _context.Add(even);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = _context.Venues.ToList();
            return View(even);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var even = await _context.Events
                .Include(e => e.Venue) 
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (even == null)
            {
                return NotFound();
            }

            return View(even);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            var even = await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);


            if (even == null)
            {
                return NotFound();
            }
            return View(even);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var even = await _context.Events.FindAsync(id);
            _context.Events.Remove(even);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var even = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (even == null)
            {
                return NotFound();
            }

            ViewBag.Venues = await _context.Venues.ToListAsync();

            return View(even);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event even)
        {
            if (id != even.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(even);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(even.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(even);
        }
    }
}
