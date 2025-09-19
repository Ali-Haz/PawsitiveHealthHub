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
    public class MedRecordsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedRecordsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MedRecords
        public async Task<IActionResult> Index(string sortOrder, string petSearch, string vetSearch, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PetSortParm"] = sortOrder == "pet_asc" ? "pet_desc" : "pet_asc";
            ViewData["CurrentPetSearch"] = petSearch;
            ViewData["CurrentVetSearch"] = vetSearch;

            var records = _context.MedRecords
    .Include(m => m.Pet)
        .ThenInclude(p => p.Owner)
    .Include(m => m.Vet)
    .AsQueryable();

            if (User.IsInRole("Owner"))
            {
                // Only show records for the pets of this owner
                var userId = _userManager.GetUserId(User);
                records = records.Where(r => r.Pet.OwnerID == userId);
            }
            else if (User.IsInRole("Vet"))
            {
                // Only show records assigned to this vet
                var userId = _userManager.GetUserId(User);
                records = records.Where(r => r.VetID == userId);
            }

            if (!string.IsNullOrEmpty(petSearch))
                records = records.Where(r => r.Pet.PetName.Contains(petSearch));

            if (!string.IsNullOrEmpty(vetSearch))
                records = records.Where(r => (r.Vet.FirstName + " " + r.Vet.LastName).Contains(vetSearch));

            records = sortOrder switch
            {
                "pet_desc" => records.OrderByDescending(r => r.Pet.PetName),
                _ => records.OrderBy(r => r.Pet.PetName)
            };

            int pageSize = 5;
            return View(await PaginatedList<MedRecords>.CreateAsync(records.AsNoTracking(), pageNumber ?? 1, pageSize));
        }




        // GET: MedRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medRecords = await _context.MedRecords
                .Include(m => m.Pet)
                .Include(m => m.Vet)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (medRecords == null)
            {
                return NotFound();
            }

            return View(medRecords);
        }

        // GET: MedRecords/Create
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create()
        {
            // Get list of Vets
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            // Get list of Owners
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

            // Send Owners list to ViewBag
            ViewBag.Owners = owners.Select(o => new SelectListItem
            {
                Value = o.Id,
                Text = o.FirstName + " " + o.LastName
            }).ToList();

            // Send Vets list to View
            ViewData["VetID"] = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName");

            return View();
        }



        // POST: MedRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create([Bind("RecordID,PetID,VetID,Diagnosis,Treatment")] MedRecords medRecords)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(medRecords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Rebuild pet name list
            ViewData["PetID"] = new SelectList(_context.Pets.Select(p => new
            {
                p.PetID,
                p.PetName
            }), "PetID", "PetName", medRecords.PetID);

            // Rebuild vet name list
            var vets = await _userManager.GetUsersInRoleAsync("Vet");
            ViewData["VetID"] = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", medRecords.VetID);

            return View(medRecords);
        }

        // GET: MedRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medRecord = await _context.MedRecords
                .Include(m => m.Pet)
                .Include(m => m.Vet)
                .FirstOrDefaultAsync(m => m.RecordID == id);

            if (medRecord == null)
            {
                return NotFound();
            }

            // Get all owners
            var owners = await _userManager.GetUsersInRoleAsync("Owner");
            ViewBag.OwnerID = new SelectList(owners.Select(o => new
            {
                o.Id,
                FullName = o.FirstName + " " + o.LastName
            }), "Id", "FullName", medRecord.Pet.OwnerID); // pre-select based on the pet's owner

            // Filter pets based on selected owner
            var pets = await _context.Pets
                .Where(p => p.OwnerID == medRecord.Pet.OwnerID)
                .ToListAsync();

            ViewBag.PetID = new SelectList(pets.Select(p => new
            {
                p.PetID,
                p.PetName
            }), "PetID", "PetName", medRecord.PetID);

            // List vets
            var vets = await _userManager.GetUsersInRoleAsync("Vet");
            ViewBag.VetID = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", medRecord.VetID);

            return View(medRecord);
        }



        // POST: MedRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Edit(int id, [Bind("RecordID,PetID,VetID,Diagnosis,Treatment")] MedRecords medRecords)
        {
            if (id != medRecords.RecordID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(medRecords);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedRecordsExists(medRecords.RecordID))
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

            // Pet dropdown with pet names
            ViewData["PetID"] = new SelectList(_context.Pets.Select(p => new
            {
                p.PetID,
                p.PetName
            }), "PetID", "PetName", medRecords.PetID);

            // Vet dropdown with names
            var vets = await _userManager.GetUsersInRoleAsync("Vet");
            ViewData["VetID"] = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", medRecords.VetID);

            return View(medRecords);
        }


        // GET: MedRecords/Delete/5
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medRecords = await _context.MedRecords
                .Include(m => m.Pet)
                .Include(m => m.Vet)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (medRecords == null)
            {
                return NotFound();
            }

            return View(medRecords);
        }

        // POST: MedRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medRecords = await _context.MedRecords.FindAsync(id);
            if (medRecords != null)
            {
                _context.MedRecords.Remove(medRecords);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedRecordsExists(int id)
        {
            return _context.MedRecords.Any(e => e.RecordID == id);
        }

        [HttpGet]
        public JsonResult GetPetsByOwner(string ownerId)
        {
            var pets = _context.Pets
                .Where(p => p.OwnerID == ownerId)
                .Select(p => new {
                    petID = p.PetID,
                    petName = p.PetName
                }).ToList();

            return Json(pets);
        }

    }
}
