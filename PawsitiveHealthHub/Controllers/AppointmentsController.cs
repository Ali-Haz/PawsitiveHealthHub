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
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Appointments.Include(a => a.Owner).Include(a => a.Pet).Include(a => a.Vet);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Owner)
                .Include(a => a.Pet)
                .Include(a => a.Vet)
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointments == null)
            {
                return NotFound();
            }

            return View(appointments);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID");
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentID,OwnerID,PetID,VetID,Date,Reason")] Appointments appointments)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(appointments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", appointments.OwnerID);
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", appointments.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", appointments.VetID);

            return View(appointments);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments.FindAsync(id);
            if (appointments == null)
            {
                return NotFound();
            }
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", appointments.OwnerID);
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", appointments.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", appointments.VetID);
            return View(appointments);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentID,OwnerID,PetID,VetID,Date,Reason")] Appointments appointments)
        {
            if (id != appointments.AppointmentID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentsExists(appointments.AppointmentID))
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
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", appointments.OwnerID);
            ViewData["PetID"] = new SelectList(_context.Pets, "PetID", "OwnerID", appointments.PetID);
            ViewData["VetID"] = new SelectList(_context.Users, "Id", "Id", appointments.VetID);
            return View(appointments);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Owner)
                .Include(a => a.Pet)
                .Include(a => a.Vet)
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointments == null)
            {
                return NotFound();
            }

            return View(appointments);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointments = await _context.Appointments.FindAsync(id);
            if (appointments != null)
            {
                _context.Appointments.Remove(appointments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentsExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentID == id);
        }
    }
}
