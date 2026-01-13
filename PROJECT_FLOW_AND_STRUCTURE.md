# HOMEOWNER Project - Flow and Structure

## 📋 Project Overview

**HOMEOWNER** is a comprehensive **Homeowners Association (HOA) Management System** built with ASP.NET Core 8.0 and Firebase Firestore. The system manages community operations including user management, facility reservations, billing, announcements, and more.

---

## 🏗️ Technical Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0 (MVC Pattern)
- **Database**: Firebase Firestore (NoSQL Cloud Database)
- **Authentication**: Cookie-based authentication with role-based access control
- **Frontend**: Razor Views with Bootstrap 5
- **Language**: C# (.NET 8.0)
- **Additional Services**: Twilio (SMS), Email (SMTP), Stripe (Payments - optional)

### Key Dependencies
```xml
- FirebaseAdmin (3.0.1)
- Google.Cloud.Firestore (3.6.0)
- Microsoft.EntityFrameworkCore (9.0.2) - for backward compatibility
- BCrypt.Net-Next (4.0.2) - password hashing
- Twilio (7.11.0) - SMS notifications
- Stripe.net (48.0.2) - payment processing
- Bootstrap & FontAwesome - UI components
```

---

## 📂 Project Structure

```
HOMEOWNER/
├── Controllers/          (20 controllers - handles HTTP requests)
│   ├── AccountController.cs           - Login/Logout/Authentication
│   ├── AdminController.cs             - Admin dashboard & management
│   ├── HomeownerController.cs         - Homeowner dashboard & actions
│   ├── DocumentController.cs          - Document management
│   ├── FacilityController.cs          - Facility management
│   ├── ReservationController.cs       - Facility reservations
│   ├── ComplaintController.cs         - Complaints & feedback
│   ├── PollController.cs              - Polls & surveys
│   ├── ForumController.cs             - Community forum
│   ├── VisitorPassController.cs       - Visitor pass management
│   ├── VehicleRegistrationController.cs - Vehicle registration
│   ├── GateAccessLogController.cs     - Security access logs
│   ├── ContactController.cs           - Contact directory
│   ├── StaffController.cs             - Staff management
│   ├── ServiceController.cs           - Service requests
│   └── ...
│
├── Models/              (32+ models - data structures)
│   ├── Admin.cs                       - Admin user model
│   ├── Homeowner.cs                   - Homeowner model
│   ├── Staff.cs                       - Staff model
│   ├── Billing.cs                     - Payment/billing model
│   ├── Announcement.cs                - Announcements model
│   ├── Facility.cs                    - Facility model
│   ├── Reservation.cs                 - Reservation model
│   ├── ServiceRequest.cs              - Service request model
│   ├── Document.cs                    - Document model
│   ├── Contact.cs                     - Contact directory model
│   ├── Poll.cs                        - Poll/survey model
│   ├── ForumPost.cs                   - Forum post model
│   ├── ForumComment.cs                - Forum comment model
│   ├── VisitorPass.cs                 - Visitor pass model
│   ├── VehicleRegistration.cs         - Vehicle model
│   ├── GateAccessLog.cs               - Access log model
│   ├── Complaint.cs                   - Complaint model
│   ├── EventModel.cs                  - Event calendar model
│   └── ViewModels/                    - View-specific models
│
├── Services/            (3 files - business logic layer)
│   ├── FirebaseService.cs             - Main data service (43KB)
│   ├── FirestoreConverters.cs         - Data conversion utilities
│   └── AsyncQueryable.cs              - Async query helpers
│
├── Views/               (16 folders - UI layers)
│   ├── Account/                       - Login, Register, Profile pages
│   ├── Admin/                         - Admin dashboard & management views
│   ├── Homeowner/                     - Homeowner dashboard & views
│   ├── Document/                      - Document management UI
│   ├── Reservation/                   - Reservation booking UI
│   ├── Forum/                         - Community forum UI
│   ├── Poll/                          - Polls & surveys UI
│   ├── Complaint/                     - Complaint submission UI
│   ├── Contact/                       - Contact directory UI
│   ├── VisitorPass/                   - Visitor pass UI
│   ├── VehicleRegistration/           - Vehicle registration UI
│   ├── GateAccessLog/                 - Access logs UI
│   ├── Service/                       - Service requests UI
│   ├── Staff/                         - Staff management UI
│   ├── Home/                          - Landing pages
│   └── Shared/                        - Layout templates, partials
│
├── Data/                (Database context - legacy SQL Server support)
│   └── ApplicationDbContext.cs
│
├── wwwroot/             (Static files)
│   ├── css/                           - Stylesheets
│   ├── js/                            - JavaScript files
│   ├── images/                        - Image assets
│   ├── uploads/                       - User uploads
│   └── lib/                           - Third-party libraries
│
├── Tools/               (Utility scripts - excluded from compilation)
│
├── Program.cs                         - Application entry point
├── appsettings.json                   - Configuration
├── HOMEOWNER.csproj                   - Project file
└── Documentation (*.md files)         - Project documentation
```

---

## 🔄 Application Flow

### 1. **Application Startup** (Program.cs)

```
1. Load configuration (appsettings.json + environment variables)
2. Configure services:
   - Firebase Firestore integration
   - Authentication (Cookie-based)
   - Session management
   - MVC controllers and views
3. Configure middleware pipeline:
   - HTTPS redirection
   - Static files serving
   - Routing
   - Authentication/Authorization
4. Start web server on ports:
   - HTTPS: https://localhost:7291
   - HTTP:  http://localhost:5020
```

### 2. **Authentication Flow**

```
User visits site
    ↓
Not authenticated → Redirect to /Account/Login
    ↓
AccountController.Login() displays login form
    ↓
User submits credentials
    ↓
AccountController validates:
    - Check if Admin, Homeowner, or Staff exists in Firebase
    - Verify password (BCrypt hashing)
    ↓
Create authentication cookie with role claims
    ↓
Redirect to appropriate dashboard:
    - Admin → /Admin/Dashboard
    - Homeowner → /Homeowner/Dashboard
    - Staff → /Staff/Dashboard
```

### 3. **User Roles & Access Control**

**Three main user types:**

#### **Admin** (Full System Access)
- Create/manage homeowners and staff accounts
- Post announcements and events
- Manage facilities and reservations
- Create and manage bills
- Upload and manage documents
- View all service requests and complaints
- Create polls and surveys
- View analytics and reports
- Manage contact directory
- View gate access logs

#### **Homeowner** (Resident Access)
- View personal dashboard
- Make facility reservations
- Submit service requests
- Submit complaints
- View and pay bills
- Request visitor passes
- Register vehicles
- View announcements and events
- Download documents
- Vote on polls
- Participate in community forum
- View contact directory

#### **Staff** (Service Provider Access)
- View assigned service requests
- Update service request status
- View facility reservations
- Log gate access entries
- View announcements

---

## 🗄️ Database Structure (Firebase Firestore)

### Collections:

```
homeowner-c355d (Firebase Project)
├── admins/                    - Admin accounts
├── homeowners/                - Homeowner accounts
├── staff/                     - Staff accounts
├── announcements/             - System announcements
├── events/                    - Event calendar
├── facilities/                - Available facilities
├── reservations/              - Facility bookings
├── bills/                     - Billing records
├── serviceRequests/           - Service request tickets
├── documents/                 - Shared documents
├── contacts/                  - Contact directory
├── polls/                     - Polls and surveys
├── pollVotes/                 - Poll voting records
├── forumPosts/                - Forum topics
├── forumComments/             - Forum replies
├── visitorPasses/             - Visitor pass requests
├── vehicleRegistrations/      - Registered vehicles
├── gateAccessLogs/            - Entry/exit logs
├── complaints/                - Homeowner complaints
└── communitySettings/         - System configuration
```

### Document Structure Example (Homeowner):
```json
{
  "HomeownerID": 1001,
  "FullName": "John Doe",
  "Email": "john@example.com",
  "PasswordHash": "hashed_password",
  "PhoneNumber": "+1234567890",
  "Address": "123 Main St, Unit 4A",
  "EmergencyContact": "Jane Doe - +1234567891",
  "Status": "Active",
  "CreatedAt": "2024-01-15T10:30:00Z"
}
```

---

## 🔑 Key Features & Workflows

### 1. **Facility Reservation Workflow**
```
Homeowner logs in
    ↓
Navigate to Reservations
    ↓
Select facility (Pool, Gym, Function Hall, etc.)
    ↓
Choose date and time slot
    ↓
Submit reservation request
    ↓
FacilityController → FirebaseService saves to Firestore
    ↓
Admin reviews in Admin Dashboard
    ↓
Admin approves/rejects reservation
    ↓
Status updated in database
    ↓
Homeowner sees updated status
```

### 2. **Service Request Workflow**
```
Homeowner submits issue (Maintenance, Security, Utilities)
    ↓
ServiceController creates ticket
    ↓
Saved to serviceRequests collection
    ↓
Admin sees request in Admin Dashboard
    ↓
Admin assigns to Staff
    ↓
Staff sees assigned request in their dashboard
    ↓
Staff updates progress (Pending → In Progress → Completed)
    ↓
Homeowner can track status in real-time
```

### 3. **Billing Workflow**
```
Admin creates bill for homeowner(s)
    ↓
Billing details saved (Monthly dues, Maintenance, Penalties)
    ↓
Homeowner views "My Bills" section
    ↓
Sees outstanding bills with due dates
    ↓
Makes payment (online or offline)
    ↓
Admin marks bill as "Paid"
    ↓
Payment history updated
```

### 4. **Announcement Distribution**
```
Admin creates announcement
    ↓
Selects category (News, Event, Emergency)
    ↓
Specifies recipient type (All, Homeowners, Staff)
    ↓
AnnouncementController saves to Firestore
    ↓
All users see announcement on dashboard
    ↓
Optional: Send email/SMS notifications (if configured)
```

---

## 🛠️ Development Workflow

### Running the Application

**Prerequisites:**
- .NET 8.0 SDK installed
- Firebase service account key downloaded
- GOOGLE_APPLICATION_CREDENTIALS environment variable set

**Start the app:**
```powershell
# From project directory
dotnet restore          # Install dependencies
dotnet build            # Compile project
dotnet run              # Start server
```

**Current Status:**
✅ Application is currently running on `http://localhost:5020`

### Creating an Admin Account

**Option 1: Firebase Console**
1. Go to Firebase Console → Firestore Database
2. Create `admins` collection
3. Add document with admin details

**Option 2: PowerShell Script**
```powershell
.\CreateAdmin.ps1
```

**Option 3: Manual C# Tool**
- Located in `/Tools` directory
- Run admin creation utility

---

## 📊 Feature Implementation Status

### ✅ **COMPLETED FEATURES (100%)**

1. ✅ User Management (Admin, Homeowner, Staff)
2. ✅ Authentication & Authorization (Role-based)
3. ✅ Admin Dashboard (Full management interface)
4. ✅ Homeowner Dashboard (Resident portal)
5. ✅ Announcements & Notifications
6. ✅ Billing & Payment Portal
7. ✅ Facility Reservation System
8. ✅ Service Request Management
9. ✅ Document Management
10. ✅ Community Forum
11. ✅ Event Calendar
12. ✅ Contact Directory
13. ✅ Visitor Pass Management
14. ✅ Vehicle Registration
15. ✅ Gate Access Logs
16. ✅ Feedback & Complaints
17. ✅ Polls & Surveys
18. ✅ Reports & Analytics
19. ✅ Mobile-Responsive Design
20. ✅ Security & Privacy (Encryption, Sessions)

### ⚠️ **OPTIONAL ENHANCEMENTS**

- Email/SMS notifications (backend ready, needs SMTP/Twilio config)
- Online payment gateway integration (Stripe SDK included)
- PDF receipt generation
- Enhanced mobile optimization

---

## 🔒 Security Features

1. **Password Security**: BCrypt hashing with salt
2. **Session Management**: 30-minute timeout, HTTP-only cookies
3. **Role-Based Access Control**: Controllers enforce authorization
4. **HTTPS Enforcement**: Redirects HTTP to HTTPS
5. **Firebase Security**: Service account authentication
6. **Input Validation**: Model validation on all forms
7. **CSRF Protection**: Anti-forgery tokens on forms

---

## 📝 Configuration Files

### appsettings.json
```json
{
  "Firebase": {
    "ProjectId": "homeowner-c355d"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587
  },
  "Twilio": {
    "AccountSid": "...",
    "AuthToken": "...",
    "PhoneNumber": "..."
  }
}
```

### Environment Variables
```
GOOGLE_APPLICATION_CREDENTIALS = Path to Firebase key JSON
Email__SmtpUser = Email username
Email__SmtpPass = Email password
Twilio__AccountSid = Twilio SID
Twilio__AuthToken = Twilio token
```

---

## 🚀 Deployment

### Production Checklist
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure Firebase security rules
- [ ] Enable HTTPS only
- [ ] Set up proper logging
- [ ] Configure secrets management
- [ ] Set up database backups
- [ ] Configure email/SMS services
- [ ] Set up monitoring and alerts

### Deployment Platforms
- **Azure App Service** (Recommended for .NET)
- **AWS Elastic Beanstalk**
- **Google Cloud Run** (Good Firebase integration)
- **Docker** (Dockerfile included)

---

## 📞 Support & Documentation

**Key Documentation Files:**
- `HOW_TO_RUN.md` - Detailed setup instructions
- `COMPLETE_SYSTEM_STATUS.md` - Feature implementation status
- `FIREBASE_SETUP.md` - Firebase configuration guide
- `QUICK_START.md` - Quick start guide
- `ADMIN_FEATURES_IMPLEMENTATION_COMPLETE.md` - Admin feature details

**Firebase Project:**
- Project ID: `homeowner-c355d`
- Console: https://console.firebase.google.com/project/homeowner-c355d

---

## 🎯 Project Highlights

**Strengths:**
✅ Clean MVC architecture
✅ Modern cloud database (Firestore)
✅ Comprehensive feature set (20 features)
✅ Role-based security
✅ Scalable Firebase backend
✅ Well-documented codebase
✅ Production-ready structure

**Current Status:**
🟢 **FULLY FUNCTIONAL** - All core features implemented and tested
🟢 **RUNNING** - Application is currently active on localhost
🟢 **READY FOR PRODUCTION** - Needs configuration for external services

---

*Last Updated: January 13, 2026*
