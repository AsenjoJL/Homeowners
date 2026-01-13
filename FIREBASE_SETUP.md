# Firebase Setup Instructions

## Prerequisites

1. **Firebase Project**: Already configured (homeowner-c355d)
2. **Service Account Key**: Required for server-side access

## Step 1: Get Firebase Service Account Key

1. Go to [Firebase Console](https://console.firebase.google.com/project/homeowner-c355d)
2. Click on the gear icon ⚙️ next to "Project Overview"
3. Select "Project settings"
4. Go to the "Service accounts" tab
5. Click "Generate new private key"
6. Download the JSON file (e.g., `homeowner-c355d-firebase-adminsdk-xxxxx.json`)
7. Save it in your project root or a secure location

## Step 2: Set Environment Variable

### Option A: Environment Variable (Recommended for Production)
```bash
# Windows PowerShell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\homeowner-c355d-firebase-adminsdk-xxxxx.json"

# Windows CMD
set GOOGLE_APPLICATION_CREDENTIALS=C:\path\to\homeowner-c355d-firebase-adminsdk-xxxxx.json

# Linux/Mac
export GOOGLE_APPLICATION_CREDENTIALS="/path/to/homeowner-c355d-firebase-adminsdk-xxxxx.json"
```

### Option B: Update Program.cs (For Development)
Update `Program.cs` to load the service account key file directly:

```csharp
// Add this before FirebaseService registration
var serviceAccountPath = builder.Configuration["Firebase:ServiceAccountPath"] 
    ?? "homeowner-c355d-firebase-adminsdk-xxxxx.json";

if (File.Exists(serviceAccountPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);
}
```

Then add to `appsettings.json`:
```json
"Firebase": {
  "ServiceAccountPath": "path/to/service-account-key.json"
}
```

## Step 3: Update FirebaseService.cs

The FirebaseService is already configured to use the project ID. Make sure it matches your Firebase project.

## Step 4: Initialize Firestore Database

Firestore will automatically create collections when you first write data. However, you may want to set up security rules in Firebase Console.

## Step 5: Test Connection

Create a test endpoint to verify Firebase connection:

```csharp
[HttpGet]
public async Task<IActionResult> TestFirebase()
{
    try
    {
        var homeowners = await _firebase.GetHomeownersAsync();
        return Ok(new { success = true, count = homeowners.Count });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, error = ex.Message });
    }
}
```

## Security Rules (Firestore)

Set up basic security rules in Firebase Console:

```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    // Allow read/write access to all documents (adjust for production)
    match /{document=**} {
      allow read, write: if request.auth != null;
    }
  }
}
```

For production, implement proper role-based access control.

## Migration Strategy

### Option 1: Gradual Migration (Recommended)
- Keep both `ApplicationDbContext` and `FirebaseService`
- Update controllers one by one
- Test each controller after migration

### Option 2: Complete Migration
- Update all controllers at once
- Remove Entity Framework dependencies
- Test thoroughly

## Current Status

✅ Firebase packages installed
✅ FirebaseService created
✅ Firebase configuration added
✅ FirebaseService registered in DI

⏳ Service account key needed
⏳ Controllers migration (in progress)

## Next Steps

1. Download service account key from Firebase Console
2. Set GOOGLE_APPLICATION_CREDENTIALS environment variable
3. Update controllers to use FirebaseService
4. Test all functionality
5. Deploy and verify

## Troubleshooting

### Error: "Could not load the default credentials"
- Solution: Set GOOGLE_APPLICATION_CREDENTIALS environment variable

### Error: "Permission denied"
- Solution: Check Firestore security rules and service account permissions

### Error: "Collection not found"
- Solution: Collections are created automatically on first write

