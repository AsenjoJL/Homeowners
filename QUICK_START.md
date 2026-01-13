# Quick Start Guide

## 🚀 Fastest Way to Run

### 1. Install .NET 8.0 SDK
Download from: https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Get Firebase Service Account Key

1. Go to: https://console.firebase.google.com/project/homeowner-c355d/settings/serviceaccounts/adminsdk
2. Click **Generate new private key**
3. Download the JSON file
4. Save it (e.g., `C:\firebase-key.json`)

### 3. Set Environment Variable

**PowerShell:**
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\firebase-key.json"
```

**CMD:**
```cmd
set GOOGLE_APPLICATION_CREDENTIALS=C:\firebase-key.json
```

### 4. Restore & Run

```bash
dotnet restore
dotnet run
```

### 5. Open Browser

Navigate to: **https://localhost:7291**

### 6. Create Admin Account

Since the database is empty, create an admin in Firebase Console:

1. Go to: https://console.firebase.google.com/project/homeowner-c355d/firestore
2. Click **Start collection**
3. Collection ID: `admins`
4. Document ID: `1`
5. Add these fields:

| Field | Type | Value |
|-------|------|-------|
| AdminID | number | 1 |
| Email | string | admin@homeowner.com |
| FullName | string | Admin User |
| PasswordHash | string | *(see below)* |
| Role | string | Admin |
| OfficeLocation | string | Main Office |
| Status | string | Active |

**To generate PasswordHash**, run this in a C# interactive window:

```csharp
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

var password = "Admin123!"; // Your password
var saltBytes = RandomNumberGenerator.GetBytes(16);
var hashBytes = KeyDerivation.Pbkdf2(
    password: password,
    salt: saltBytes,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: 100000,
    numBytesRequested: 32);
var hash = $"{Convert.ToBase64String(saltBytes)}:{Convert.ToBase64String(hashBytes)}";
Console.WriteLine(hash);
```

Copy the output and paste it as the PasswordHash value.

### 7. Login

1. Go to: https://localhost:7291/Account/Login
2. Email: `admin@homeowner.com`
3. Password: `Admin123!` (or whatever you used)

## ✅ That's It!

You should now be logged in and see the Admin Dashboard.

## Common Issues

**"Could not load the default credentials"**
→ Make sure `GOOGLE_APPLICATION_CREDENTIALS` is set correctly

**"Permission denied"**
→ Check Firebase Console → Firestore → Rules (allow read/write temporarily for testing)

**Port already in use**
→ Change ports in `Properties/launchSettings.json` or kill the process

## Next Steps

- Create homeowners
- Add facilities
- Create staff members
- Test reservations
- Test service requests

For detailed instructions, see `HOW_TO_RUN.md`

