# FlexiSpace Booking System

## WIL Group Information

**WIL Group number :** 16

### Group Members

| Name | ST Number | Allocated ARC Group |
|---|---|---|
| Khensani Maluleke | ST10451309 | 2 |
| Khumo-Thato Chabeli | ST10448834 | 3 |
| Orearabetse Riba | ST10446648 | 2|
| Denzel Tinotenda Pawandiwa | ST10457186| 2|
| Tinotenda Nyakatsari | ST10437488 | 2|

---

## Project Overview

FlexiSpace is a room and boardroom booking system designed to make the process of finding, booking and managing meeting spaces easier.

The system consists of a web application and a mobile application connected to a shared backend API and database.

---

## Repository Contents

### Task 1
Contains the completed Task 1 assessment documentation and supporting diagrams.

### Prototype
Contains the project prototypes and instructions for accessing the mobile and web applications.

### Attendance
Contains the team's meeting attendance evidence and supporting documentation.

---

## Prototype Access

### Mobile app (available now)

The .NET MAUI mobile prototype lives in `Flexispace Mobile/Flexispace.Mobile/`. It runs against mock services, so no backend or database setup is required.

#### Prerequisites

- Windows 10 or later (recommended for this demo)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- .NET MAUI workload:

```powershell
dotnet workload install maui
```

#### Setup and run

1. Clone this repository.
2. Open a terminal in the project folder:

```powershell
cd "Flexispace Mobile\Flexispace.Mobile"
dotnet restore
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

3. On the welcome screen, tap **Get a quote · Book a room**, then sign in with one of the demo accounts below (or tap a role tile on the login screen).

#### Demo accounts

All demo passwords are `demo123`.

| Email | Role | What you can test |
|---|---|---|
| `staff@flexispace.net.za` | Staff | Book rooms, view availability, manage own bookings |
| `client@example.com` | Client | Request bookings (pending approval flow) |
| `rebecca@flexispace.net.za` | Centre Manager | Approve/decline bookings, block rooms, location reports |
| `antoinette@flexispace.net.za` | Centre Manager | Same as above for Eagle Canyon |
| `admin@flexispace.net.za` | Administrator | Full manage console, users, rooms, and system-wide reports |

Each role gets its own tab bar and screens — not the same UI with hidden buttons.

#### Notes for assessors

- Locations included: Centurion, Houghton Estate, and Eagle Canyon.
- On Windows, a floating help button (bottom-right) opens the prototype FAQ assistant.
- API shapes for the future backend are documented in `Flexispace Mobile/Flexispace.Mobile/API_CONTRACT.md`.

### Web app

The web prototype is not included in this repository yet. It will be added under the **Prototype** section when available.


## Assessment Submission

All assessment documents for the project are stored in this repository.

The GitHub repository link will be submitted through ARC as required.
