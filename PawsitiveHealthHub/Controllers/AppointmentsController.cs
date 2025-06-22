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
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create()
        {
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            ViewData["VetID"] = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName");

            // Only show pets that belong to the current user
            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["PetID"] = new SelectList(_context.Pets
                .Where(p => p.OwnerID == currentUser.Id)
                .Select(p => new { p.PetID, p.PetName }),
                "PetID", "PetName");

            return View();
        }



        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create([Bind("AppointmentID,PetID,VetID,Date,Reason")] Appointments appointments)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            appointments.OwnerID = currentUser.Id; // Automatically set OwnerID

            if (!ModelState.IsValid)
            {
                _context.Add(appointments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            ViewData["VetID"] = new SelectList(vets.Select(v => new
            {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", appointments.VetID);

            ViewData["PetID"] = new SelectList(_context.Pets
                .Where(p => p.OwnerID == currentUser.Id)
                .Select(p => new { p.PetID, p.PetName }),
                "PetID", "PetName", appointments.PetID);

            return View(appointments);
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointments = await _context.Appointments.FindAsync(id);
            if (appointments == null)
                return NotFound();

            var owners = await _userManager.GetUsersInRoleAsync("Owner");
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            ViewData["OwnerID"] = new SelectList(owners.Select(o => new {
                o.Id,
                FullName = o.FirstName + " " + o.LastName
            }), "Id", "FullName", appointments.OwnerID);

            ViewData["VetID"] = new SelectList(vets.Select(v => new {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", appointments.VetID);

            ViewData["PetID"] = new SelectList(_context.Pets.Select(p => new {
                p.PetID,
                p.PetName
            }), "PetID", "Name", appointments.PetID);
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
                    return RedirectToAction(nameof(Index));
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
            }

            var owners = await _userManager.GetUsersInRoleAsync("Owner");
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            ViewData["OwnerID"] = new SelectList(owners.Select(o => new {
                o.Id,
                FullName = o.FirstName + " " + o.LastName
            }), "Id", "FullName", appointments.OwnerID);

            ViewData["VetID"] = new SelectList(vets.Select(v => new {
                v.Id,
                FullName = "Dr. " + v.LastName
            }), "Id", "FullName", appointments.VetID);

            ViewData["PetID"] = new SelectList(_context.Pets.Select(p => new {
                p.PetID,
                p.PetName
            }), "PetID", "PetName", appointments.PetID);

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
