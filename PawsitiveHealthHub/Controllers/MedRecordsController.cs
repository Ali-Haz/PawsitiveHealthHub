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
                .Include(m => m.Vet)
                .AsQueryable();

            if (!string.IsNullOrEmpty(petSearch))
            {
                records = records.Where(r => r.Pet.PetName.Contains(petSearch));
            }

            if (!string.IsNullOrEmpty(vetSearch))
            {
                records = records.Where(r => (r.Vet.FirstName + " " + r.Vet.LastName).Contains(vetSearch));
            }

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
        public async Task<IActionResult> Create()
        {
            // Get list of vets only
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            // Set pet names
            ViewData["PetID"] = new SelectList(_context.Pets.Select(p => new {
                p.PetID,
                p.PetName
            }), "PetID", "PetName");

            // Set vet names
            ViewData["VetID"] = new SelectList(vets.Select(v => new {
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

            var medRecords = await _context.MedRecords.FindAsync(id);
            if (medRecords == null)
            {
                return NotFound();
            }

            // Pet dropdown with names
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


        // POST: MedRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
