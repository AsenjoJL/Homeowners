# Firebase Migration Guide

## Overview
This project has been migrated from SQL Server (Entity Framework Core) to Firebase Firestore.

## Firebase Configuration
- **Project ID**: homeowner-c355d
- **API Key**: Configured in appsettings.json
- **Database**: Firestore (NoSQL)

## Key Changes

### 1. Data Access Layer
- **Old**: `ApplicationDbContext` (Entity Framework Core)
- **New**: `FirebaseService` (Firestore)

### 2. Models
All models remain the same, but Firestore handles serialization automatically.

### 3. Controllers
Controllers can now use `FirebaseService` instead of `ApplicationDbContext`. The service provides async methods for all CRUD operations.

## Firebase Service Usage

### Dependency Injection
```csharp
private readonly FirebaseService _firebase;

public MyController(FirebaseService firebase)
{
    _firebase = firebase;
}
```

### Example: Get Homeowners
```csharp
// Old (EF Core)
var homeowners = _context.Homeowners.ToList();

// New (Firebase)
var homeowners = await _firebase.GetHomeownersAsync();
```

### Example: Get by Email
```csharp
// Old (EF Core)
var homeowner = await _context.Homeowners.FirstOrDefaultAsync(h => h.Email == email);

// New (Firebase)
var homeowner = await _firebase.GetHomeownerByEmailAsync(email);
```

### Example: Add Entity
```csharp
// Old (EF Core)
_context.Homeowners.Add(homeowner);
await _context.SaveChangesAsync();

// New (Firebase)
await _firebase.AddHomeownerAsync(homeowner);
```

## Collection Names in Firestore
- `homeowners` - Homeowner records
- `admins` - Admin records
- `staff` - Staff records
- `facilities` - Facility records
- `reservations` - Reservation records
- `serviceRequests` - Service request records
- `announcements` - Announcement records
- `forumPosts` - Forum post records
- `forumComments` - Forum comment records
- `reactions` - Reaction records
- `events` - Event records
- `notifications` - Notification records
- `communitySettings` - Community settings
- `homeownerProfileImages` - Profile images

## Important Notes

1. **Async Operations**: All Firebase operations are async. Make sure to use `await`.

2. **ID Generation**: Firestore uses string IDs. The service converts integer IDs to strings for document IDs.

3. **DateTime Handling**: Firestore stores DateTime as Timestamp. The Firebase SDK handles conversion automatically.

4. **TimeSpan**: TimeSpan values are stored as strings in Firestore (e.g., "01:30:00").

5. **Navigation Properties**: Firestore doesn't support navigation properties like EF Core. Related data must be loaded separately.

6. **Queries**: Use the provided query methods in FirebaseService. Complex LINQ queries won't work the same way.

## Migration Steps

1. ✅ Firebase packages installed
2. ✅ FirebaseService created
3. ✅ Firebase configuration added to appsettings.json
4. ✅ FirebaseService registered in Program.cs
5. ⏳ Update controllers to use FirebaseService (in progress)
6. ⏳ Test all functionality
7. ⏳ Remove SQL Server dependencies (optional)

## Next Steps

To complete the migration:
1. Update each controller to inject `FirebaseService` instead of `ApplicationDbContext`
2. Replace EF Core queries with FirebaseService method calls
3. Update all async operations to use Firebase methods
4. Test thoroughly
5. Remove Entity Framework Core packages if no longer needed

## Firebase Console
Access your Firebase console at: https://console.firebase.google.com/project/homeowner-c355d

## Authentication
Firebase Authentication can be integrated separately. Currently using Cookie Authentication which works independently.

