# Quick Firebase Setup

## The Error You're Seeing

```
Your default credentials were not found. To set up Application Default Credentials...
```

This means Firebase can't find your service account key file.

## Quick Fix (3 Steps)

### Step 1: Get Firebase Service Account Key

1. Go to: https://console.firebase.google.com/project/homeowner-c355d/settings/serviceaccounts/adminsdk
2. Click **"Generate new private key"**
3. Click **"Generate key"** in the popup
4. Download the JSON file (e.g., `homeowner-c355d-firebase-adminsdk-xxxxx.json`)
5. Save it somewhere safe (e.g., `C:\firebase-keys\`)

### Step 2: Set Environment Variable

**Option A: PowerShell (Current Session Only)**
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json"
```

**Option B: Use the Setup Script**
```powershell
.\SETUP_FIREBASE.ps1
```

**Option C: Set Permanently (Windows)**
1. Press `Win + R`, type `sysdm.cpl`, press Enter
2. Click **"Environment Variables"**
3. Under **"User variables"**, click **"New"**
4. Variable name: `GOOGLE_APPLICATION_CREDENTIALS`
5. Variable value: `C:\firebase-keys\your-file-name.json`
6. Click **OK** on all dialogs
7. **Restart your terminal/IDE**

### Step 3: Verify and Run

**Verify it's set:**
```powershell
echo $env:GOOGLE_APPLICATION_CREDENTIALS
```

**Run the application:**
```powershell
dotnet run
```

## Troubleshooting

### Still Getting the Error?

1. **Check the path is correct:**
   ```powershell
   Test-Path $env:GOOGLE_APPLICATION_CREDENTIALS
   ```
   Should return `True`

2. **Check the file exists:**
   ```powershell
   Get-Item $env:GOOGLE_APPLICATION_CREDENTIALS
   ```

3. **Restart your terminal/IDE** after setting permanently

4. **Try setting it again in the current session:**
   ```powershell
   $env:GOOGLE_APPLICATION_CREDENTIALS="C:\full\path\to\your-file.json"
   dotnet run
   ```

### File Not Found?

Make sure:
- The file path has no spaces (or wrap in quotes)
- The file extension is `.json`
- You have read permissions on the file
- The path uses backslashes `\` on Windows

### Still Having Issues?

Check the file is valid JSON:
```powershell
Get-Content $env:GOOGLE_APPLICATION_CREDENTIALS | ConvertFrom-Json
```

Should show JSON content without errors.

## Next Steps

After setting up Firebase credentials:
1. ✅ Run `dotnet run`
2. ✅ Application should start without Firebase errors
3. ⏳ Create admin account in Firebase (see `QUICK_START.md`)
4. ⏳ Login and test the system

