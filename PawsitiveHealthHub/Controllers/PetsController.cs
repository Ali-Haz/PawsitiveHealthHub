using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PawsitiveHealthHub.Areas.Identity.Data;
using PawsitiveHealthHub.Models;

namespace PawsitiveHealthHub.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Pets
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["CurrentFilter"] = searchString;

            var pets = _context.Pets.Include(p => p.Owner).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                pets = pets.Where(p => p.PetName.Contains(searchString));

            pets = sortOrder switch
            {
                "name_desc" => pets.OrderByDescending(p => p.PetName),
                _ => pets.OrderBy(p => p.PetName)
            };

            int pageSize = 5;
            return View(await PaginatedList<Pets>.CreateAsync(pets.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.PetID == id);
            if (pets == null)
            {
                return NotFound();
            }

            return View(pets);
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetID,PetName,Species,AgeYears,AgeMonths,AgeDays")] Pets pets)
        {
            if (!ModelState.IsValid)
            {
                // Get the current logged-in user
                var currentUserId = _userManager.GetUserId(User);

                // Set the OwnerID to the current user's ID
                pets.OwnerID = currentUserId;

                _context.Add(pets);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(pets);
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            // Sets the OwnerID to the current user's ID
            pet.OwnerID = currentUser.Id;

            // Owner full name is displayed
            ViewData["OwnerFullName"] = currentUser.FirstName + " " + currentUser.LastName;

            return View(pet);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PetID,OwnerID,PetName,Species,AgeYears,AgeMonths,AgeDays")] Pets pets)
        {
            if (id != pets.PetID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(pets);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsExists(pets.PetID))
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
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", pets.OwnerID);
            return View(pets);
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.PetID == id);
            if (pets == null)
            {
                return NotFound();
            }

            return View(pets);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pets = await _context.Pets.FindAsync(id);
            if (pets != null)
            {
                _context.Pets.Remove(pets);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetsExists(int id)
        {
            return _context.Pets.Any(e => e.PetID == id);
        }
    }
}
