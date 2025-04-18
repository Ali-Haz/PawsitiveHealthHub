using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PawsitiveHealthHub.Models;

namespace PawsitiveHealthHub.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    // Navigation Properties
    public ICollection<Pets> Pets { get; set; }
    public ICollection<Appointments> OwnerAppointments { get; set; }
    public ICollection<Appointments> VetAppointments { get; set; }
    public ICollection<MedRecords> VetMedRecords { get; set; }

}
