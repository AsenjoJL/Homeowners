# How to Run the HOMEOWNER System

## Prerequisites

1. **.NET 8.0 SDK** - Download from https://dotnet.microsoft.com/download/dotnet/8.0
2. **Visual Studio 2022** (or VS Code) - Recommended IDE
3. **Firebase Project** - Already configured (homeowner-c355d)
4. **Firebase Service Account Key** - Required for database access

## Step 1: Install Dependencies

Open PowerShell or Command Prompt in the project directory and run:

```bash
dotnet restore
```

This will install all NuGet packages including:
- Firebase Admin SDK
- Google Cloud Firestore
- ASP.NET Core packages

## Step 2: Set Up Firebase Service Account

### Option A: Using Environment Variable (Recommended)

1. Go to [Firebase Console](https://console.firebase.google.com/project/homeowner-c355d)
2. Click ⚙️ (Settings) → **Project settings**
3. Go to **Service accounts** tab
4. Click **Generate new private key**
5. Download the JSON file (e.g., `homeowner-c355d-firebase-adminsdk-xxxxx.json`)
6. Save it in a secure location (e.g., `C:\firebase-keys\`)

**Set Environment Variable:**

**Windows PowerShell:**
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json"
```

**Windows CMD:**
```cmd
set GOOGLE_APPLICATION_CREDENTIALS=C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json
```

**Linux/Mac:**
```bash
export GOOGLE_APPLICATION_CREDENTIALS="/path/to/service-account-key.json"
```

### Option B: Update Program.cs (For Development)

Alternatively, you can hardcode the path in `Program.cs`:

```csharp
// Add this before FirebaseService registration
var serviceAccountPath = @"C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json";
if (File.Exists(serviceAccountPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);
}
```

## Step 3: Configure Environment Variables (Optional)

If you're using email or Twilio services, set these environment variables:

**Windows PowerShell:**
```powershell
$env:Email__SmtpHost="smtp.gmail.com"
$env:Email__SmtpUser="your-email@gmail.com"
$env:Email__SmtpPass="your-app-password"
$env:Twilio__AccountSid="your-twilio-sid"
$env:Twilio__AuthToken="your-twilio-token"
$env:Twilio__PhoneNumber="your-twilio-number"
```

**Windows CMD:**
```cmd
set Email__SmtpHost=smtp.gmail.com
set Email__SmtpUser=your-email@gmail.com
set Email__SmtpPass=your-app-password
set Twilio__AccountSid=your-twilio-sid
set Twilio__AuthToken=your-twilio-token
set Twilio__PhoneNumber=your-twilio-number
```

## Step 4: Build the Project

```bash
dotnet build
```

Fix any compilation errors if they occur.

## Step 5: Run the Application

### Option A: Using .NET CLI

```bash
dotnet run
```

The application will start and show output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7291
      Now listening on: http://localhost:5020
```

### Option B: Using Visual Studio

1. Open `HOMEOWNER.sln` in Visual Studio
2. Press **F5** or click **Start Debugging**
3. The application will launch in your default browser

### Option C: Using Visual Studio Code

1. Open the project folder in VS Code
2. Press **F5** or go to **Run → Start Debugging**
3. Select **.NET Core** configuration

## Step 6: Access the Application

Open your browser and navigate to:
- **HTTPS**: https://localhost:7291
- **HTTP**: http://localhost:5020

## Step 7: Initial Setup - Create Admin Account

Since the database is empty, you'll need to create an admin account. You have two options:

### Option A: Use Firebase Console

1. Go to [Firebase Console](https://console.firebase.google.com/project/homeowner-c355d/firestore)
2. Create a collection called `admins`
3. Add a document with these fields:
   ```json
   {
     "AdminID": 1,
     "FullName": "Admin User",
     "Email": "admin@example.com",
     "PasswordHash": "<hashed-password>",
     "Role": "Admin",
     "OfficeLocation": "Main Office",
     "Status": "Active"
   }
   ```

**To hash a password**, you can use this C# code or create a simple console app:

```csharp
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

var password = "Admin123!"; // Change this
var saltBytes = RandomNumberGenerator.GetBytes(16);
var hashBytes = KeyDerivation.Pbkdf2(
    password: password,
    salt: saltBytes,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: 100000,
    numBytesRequested: 32);
var hash = $"{Convert.ToBase64String(saltBytes)}:{Convert.ToBase64String(hashBytes)}";
Console.WriteLine($"PasswordHash: {hash}");
```

### Option B: Create Seed Data Script

Create a file `SeedData.cs` and run it once to populate initial data.

## Step 8: Test Login

1. Navigate to `/Account/Login`
2. Use the admin credentials you created
3. You should be redirected to the Admin Dashboard

## Troubleshooting

### Error: "Could not load the default credentials"

**Solution:** Make sure `GOOGLE_APPLICATION_CREDENTIALS` environment variable is set correctly.

```powershell
# Verify it's set
echo $env:GOOGLE_APPLICATION_CREDENTIALS

# If not set, set it again
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\service-account-key.json"
```

### Error: "Permission denied" or "Access denied"

**Solution:** 
1. Check Firebase Console → Firestore Database → Rules
2. Temporarily allow all access for testing:
   ```javascript
   rules_version = '2';
   service cloud.firestore {
     match /databases/{database}/documents {
       match /{document=**} {
         allow read, write: if true; // TEMPORARY - Change for production!
       }
     }
   }
   ```

### Error: "Collection not found"

**Solution:** Collections are created automatically when you first write data. This is normal.

### Error: Port already in use

**Solution:** Change the port in `Properties/launchSettings.json` or kill the process using the port:

```powershell
# Find process using port 5020 or 7291
netstat -ano | findstr :5020
netstat -ano | findstr :7291

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### Application won't start

**Solution:**
1. Check if .NET 8.0 SDK is installed: `dotnet --version`
2. Restore packages: `dotnet restore`
3. Clean and rebuild: `dotnet clean && dotnet build`

## Development Tips

1. **Hot Reload**: The application supports hot reload. Make changes and save - they'll be reflected automatically.

2. **View Logs**: Check the console output for detailed error messages.

3. **Firebase Console**: Monitor your Firestore database in real-time at https://console.firebase.google.com/project/homeowner-c355d/firestore

4. **Debug Mode**: Use Visual Studio debugger to set breakpoints and inspect variables.

## Production Deployment

For production:

1. Set `ASPNETCORE_ENVIRONMENT=Production`
2. Configure proper Firebase security rules
3. Use HTTPS only
4. Set up proper error logging
5. Configure connection strings and secrets securely
6. Use Azure App Service, AWS, or similar hosting platform

## Quick Start Checklist

- [ ] .NET 8.0 SDK installed
- [ ] Dependencies restored (`dotnet restore`)
- [ ] Firebase service account key downloaded
- [ ] `GOOGLE_APPLICATION_CREDENTIALS` environment variable set
- [ ] Project builds successfully (`dotnet build`)
- [ ] Application runs (`dotnet run`)
- [ ] Can access https://localhost:5001
- [ ] Admin account created in Firestore
- [ ] Can login successfully

## Next Steps

After running the system:
1. Create admin account
2. Create test homeowners
3. Create facilities
4. Test reservations
5. Test service requests
6. Test forum functionality

## Support

If you encounter issues:
1. Check the console logs for error messages
2. Verify Firebase service account key is correct
3. Check Firestore security rules
4. Ensure all environment variables are set
5. Review `FIREBASE_SETUP.md` for detailed Firebase configuration

