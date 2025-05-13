using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public MedRecordsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MedRecords
        public async Task<IActionResult> Index(int? petId)
        {
            var records = _context.MedRecords
                .Include(m => m.Pet)
                .Include(m => m.Vet)
                .AsQueryable();

            if (petId.HasValue)
            {
                records = records.Where(m => m.PetID == petId.Value);
            }

            return View(await records.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID");
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id");
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
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", medRecords.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", medRecords.VetID);
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
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", medRecords.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", medRecords.VetID);
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
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", medRecords.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", medRecords.VetID);
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
