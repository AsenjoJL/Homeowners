# Admin Features - SQL Server vs Firebase Issues

## 📊 **Status Summary:**

| Feature | Controller | Database | Status |
|---------|------------|----------|--------|
| Add Facility | FacilityController | ❌ SQL Server | 🔴 Not Working |
| Add Event | AdminController | ❌ SQL Server | 🔴 Not Working |
| Add Announcement | AdminController | ✅ Firebase | 🟢 Should Work |
| Add Homeowner | AdminController | ✅ Firebase | 🟢 Working |
| Add Staff | AdminController | ✅ Firebase | 🟢 Working |
| Manage Billing | AdminController | ✅ Firebase | 🟢 Working |

---

## ❌ **NOT WORKING:**

### **1. Add Facility**
- **File:** `Controllers/FacilityController.cs`
- **Problem:** Uses `ApplicationDbContext` (SQL Server)
- **Line:** 87 - `_context.Facilities.Add(facility)`

### **2. Add Event**
- **File:** `Controllers/AdminController.cs`
- **Problem:** Uses `SqlConnection` directly (SQL Server)
- **Lines:** 399-413 - Direct SQL INSERT command
```csharp
using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("HOME_DB")))
{
    SqlCommand cmd = new SqlCommand(@"INSERT INTO Events...", conn);
    // ...
}
```

---

## ✅ **WORKING:**

### **3. Add Announcement**
- **File:** `Controllers/AdminController.cs`
- **Uses:** Firebase (`_data.AddAnnouncementAsync`)
- **Lines:** 564-610
- **Status:** ✅ Should work perfectly!

---

## 🔧 **Solution:**

### **Option 1: Quick Fix - Disable SQL Server Features**
Comment out SQL Server code for Facility and Event:

#### **For Events (AdminController.cs):**
```csharp
[HttpPost]
public IActionResult AddEvent(EventModel model)
{
    // TEMPORARILY DISABLED - Uses SQL Server
    return Json(new { success = false, message = "Event management temporarily unavailable. Firebase implementation pending." });
}
```

#### **For Facilities (FacilityController.cs):**
```csharp
[HttpPost]
public async Task<IActionResult> Add(Facility facility, List<IFormFile> ImageFiles)
{
    // TEMPORARILY DISABLED - Uses SQL Server
    return Json(new { success = false, message = "Facility management temporarily unavailable. Firebase implementation pending." });
}
```

### **Option 2: Implement Firebase (Recommended)**
Rewrite Event and Facility management to use Firebase like Announcements do.

---

## 📝 **Detailed Analysis:**

### **Events - Uses SQL Server:**
```csharp
// Line 399-413 in AdminController.cs
using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("HOME_DB")))
{
    SqlCommand cmd = new SqlCommand(@"INSERT INTO Events (Title, Description, EventDate, Category, CreatedBy, Location)
                   VALUES (@Title, @Description, @EventDate, @Category, @CreatedBy, @Location)", conn);
    
    cmd.Parameters.AddWithValue("@Title", model.Title);
    cmd.Parameters.AddWithValue("@Description", model.Description ?? string.Empty);
    cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
    cmd.Parameters.AddWithValue("@Category", model.Category ?? "General");
    cmd.Parameters.AddWithValue("@CreatedBy", GetCurrentAdminID());
    cmd.Parameters.AddWithValue("@Location", model.Location ?? "Not set");
    
    conn.Open();
    cmd.ExecuteNonQuery();
}
```

### **Announcements - Uses Firebase (Working!):**
```csharp
// Line 570-578 in AdminController.cs
var announcement = new Announcement
{
    Title = model.Title,
    Content = model.Content,
    PostedAt = DateTime.Now,
    IsUrgent = model.IsUrgent
};

await _data.AddAnnouncementAsync(announcement); // ✅ Firebase!
```

---

## 🎯 **Recommendation:**

**I recommend Option 1 (Quick Fix)** for now:
1. Disable Event and Facility features temporarily
2. Show "temporarily unavailable" message
3. Implement Firebase properly later

This is consistent with what we did for Reservations and Service Requests.

---

## 📋 **Controllers Using SQL Server:**

1. ❌ `FacilityController.cs` - Uses `ApplicationDbContext`
2. ❌ `AdminController.cs` - Uses `SqlConnection` for Events
3. ❌ `ReservationController.cs` - Uses `ApplicationDbContext` (already disabled)
4. ❌ `ServiceController.cs` - Uses `ApplicationDbContext`
5. ❌ `ForumController.cs` - Uses `ApplicationDbContext`
6. ❌ `ManageOwnersController.cs` - Uses `ApplicationDbContext`

---

## ✅ **Controllers Using Firebase:**

1. ✅ `AdminController.cs` - Uses `IDataService` for Announcements, Homeowners, Staff, Billing
2. ✅ `HomeownerController.cs` - Uses `IDataService`
3. ✅ `ComplaintController.cs` - Uses `IDataService`
4. ✅ `VehicleRegistrationController.cs` - Uses `IDataService`
5. ✅ `VisitorPassController.cs` - Uses `IDataService`
6. ✅ `PollController.cs` - Uses `IDataService`

---

**Would you like me to apply the Quick Fix (disable Events and Facilities)?**
