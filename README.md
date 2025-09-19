# PawsitiveHealthHub
Pawsitive Health Hub is a web application made for pet owners and veterinarians. Users can stay updated on their pet's health and easily contact vets when needed. Built with ASP.NET Core MVC and Entity Framework Core, this application offers features like pet profiles, appointment scheduling, and medical record tracking.

---

# Features

 # Owner Users
- Create, view, and manage pet profiles.
- Book appointments with vets.
- View medical records for their pets.

# Vet Users
- Create, edit, and delete medical records.

# Authentication & Roles
- Secure login and registration using ASP.NET Core Identity
- Role-based access:
  - Owners: Can create, edit, and delete pet profiles and schedule appointments.
  - Vets: Can create, edit, and delete medicale records

---

# Tech Stack

- .NET 8.0 with ASP.NET Core MVC
- Entity Framework Core (SQL Server)
- Bootstrap 5 for UI styling
- Identity for authentication & authorization

---

# How It Works

- When registrating, users are assigned the Owner role.
- Owners can add pets and book appointments.
- Vets can manage pet medical records and see pet owners' details only when needed.
- Search, sorting, and pagination features are available for all major tables.

---

# UI Design

- Uses a color scheme of dark blue, black, and white, with both dark and light text interface.
- Consistent layout across all Pages
- Hover effects
- Clean navigation bar (with role-based visibility)

---

# User Profiles

Use the following accounts to log in as **Vet** users:

Email: vet1@hub.com
Password: Vet123!

Email: vet2@hub.com
Password: Vet456!

Email: vet3@hub.com
Password: Vet789!

Use the following accounts to log in as **Owner** users:

Email: owner1@hub.com
Password: Owner123!

Email: MayBaxter@test.com
Password: May123!

Email: NancyGreen@test.com
Password: Nancy123!

