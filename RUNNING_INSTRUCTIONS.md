# Running Instructions

## ✅ Application Status

The application should now be running!

## Access the Application

Open your browser and go to:
- **HTTP:** http://localhost:5020
- **HTTPS:** https://localhost:7291

## Before You Can Login

You need to create an admin account in Firebase first:

### Quick Method: Use the Tool

```powershell
dotnet run --project Tools/CreateAdminTool.csproj
```

This will generate the admin account data. Then:

1. Go to: https://console.firebase.google.com/project/homeowner-c355d/firestore
2. Create collection: `admins`
3. Add document ID: `1`
4. Add the fields shown by the tool
5. Save

### Login After Creating Admin

1. Go to: http://localhost:5020/Account/Login
2. Email: `admin@homeowner.com`
3. Password: `Admin123!`

## Stopping the Application

To stop the running application:
- Press **Ctrl+C** in the terminal where it's running
- Or close the terminal window

## If You Get "File Locked" Error

If you see an error about the file being locked:

```powershell
# Stop any running instances
Get-Process -Name "HOMEOWNER" -ErrorAction SilentlyContinue | Stop-Process -Force

# Then run again
dotnet run
```

## Common Issues

### Port Already in Use

If port 5020 or 7291 is already in use:
- Change ports in `Properties/launchSettings.json`
- Or stop the process using the port

### Firebase Errors

If you get Firebase credential errors:
1. Check: `echo $env:GOOGLE_APPLICATION_CREDENTIALS`
2. Verify file exists: `Test-Path $env:GOOGLE_APPLICATION_CREDENTIALS`
3. Re-run: `.\FIND_FIREBASE_KEY.ps1`

## Next Steps

1. ✅ Application is running
2. ⏳ Create admin account in Firebase
3. ⏳ Login and test the system
4. ⏳ Create test data (homeowners, facilities, etc.)

## You're All Set! 🎉

The application is running. Just create the admin account and you can start using it!

