# How to Get Firebase Service Account Key

## Step-by-Step Instructions

### Step 1: Go to Firebase Console

Open this URL in your browser:
**https://console.firebase.google.com/project/homeowner-c355d/settings/serviceaccounts/adminsdk**

Or navigate manually:
1. Go to: https://console.firebase.google.com/
2. Select project: **homeowner-c355d**
3. Click the ⚙️ **Settings** icon (top left)
4. Click **"Project settings"**
5. Go to the **"Service accounts"** tab

### Step 2: Generate Service Account Key

1. On the **Service accounts** tab, you'll see:
   - **Node.js** tab (default)
   - **Python** tab
   - **Java** tab
   - etc.

2. Look for the section that says:
   ```
   Generate new private key
   ```

3. Click the **"Generate new private key"** button

4. A popup will appear warning about keeping the key secure
   - Click **"Generate key"** to confirm

5. A JSON file will automatically download to your **Downloads** folder
   - File name will be something like: `homeowner-c355d-firebase-adminsdk-xxxxx-xxxxxxxxxx.json`
   - The `xxxxx` parts are random characters

### Step 3: Save the File

1. **Find the downloaded file** in your Downloads folder
2. **Move it to a safe location**, for example:
   - `C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json`
   - Or `C:\Users\John Lester\Desktop\HOMEOWNER\firebase-key.json`

3. **Create the folder if needed:**
   ```powershell
   New-Item -ItemType Directory -Path "C:\firebase-keys" -Force
   ```

4. **Move the file:**
   ```powershell
   Move-Item "$env:USERPROFILE\Downloads\homeowner-c355d-firebase-adminsdk-*.json" "C:\firebase-keys\"
   ```

### Step 4: Set the Environment Variable

**Option A: For Current PowerShell Session (Temporary)**
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json"
```

Replace `xxxxx` with the actual characters in your filename.

**Option B: Use the Setup Script**
```powershell
.\SETUP_FIREBASE.ps1
```

This script will:
- Ask you for the file path
- Set the environment variable
- Optionally set it permanently

**Option C: Set Permanently (Windows)**

1. Press `Win + R`
2. Type: `sysdm.cpl` and press Enter
3. Click **"Environment Variables"** button
4. Under **"User variables"**, click **"New"**
5. Variable name: `GOOGLE_APPLICATION_CREDENTIALS`
6. Variable value: `C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json`
7. Click **OK** on all dialogs
8. **Restart your terminal/IDE** for it to take effect

### Step 5: Verify It's Set

Run this command to check:
```powershell
echo $env:GOOGLE_APPLICATION_CREDENTIALS
```

It should show your file path.

Also verify the file exists:
```powershell
Test-Path $env:GOOGLE_APPLICATION_CREDENTIALS
```

Should return `True`.

## Quick Visual Guide

```
Firebase Console
    ↓
Project Settings (⚙️ icon)
    ↓
Service accounts tab
    ↓
"Generate new private key" button
    ↓
Click "Generate key" in popup
    ↓
JSON file downloads
    ↓
Save to safe location (e.g., C:\firebase-keys\)
    ↓
Set environment variable
    ↓
Done! ✅
```

## What the File Looks Like

The downloaded JSON file contains something like:
```json
{
  "type": "service_account",
  "project_id": "homeowner-c355d",
  "private_key_id": "xxxxx",
  "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
  "client_email": "firebase-adminsdk-xxxxx@homeowner-c355d.iam.gserviceaccount.com",
  "client_id": "xxxxx",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  ...
}
```

**⚠️ SECURITY WARNING:**
- Keep this file **SECRET** and **PRIVATE**
- Never commit it to Git
- Never share it publicly
- It gives full access to your Firebase project

## Troubleshooting

### Can't Find "Generate new private key" Button?

Make sure you're:
1. On the correct project (homeowner-c355d)
2. In **Project settings** → **Service accounts** tab
3. Logged in with the correct Google account

### File Not Downloading?

- Check your browser's download settings
- Check Downloads folder
- Try a different browser
- Check if popup blocker is preventing the download

### Still Having Issues?

1. Make sure you have **Owner** or **Editor** permissions on the Firebase project
2. Try refreshing the Firebase Console page
3. Clear browser cache and try again

## After Getting the Key

Once you have the key file:

1. ✅ Set the environment variable (see Step 4 above)
2. ✅ Verify it's set: `echo $env:GOOGLE_APPLICATION_CREDENTIALS`
3. ✅ Run your application: `dotnet run`
4. ✅ The Firebase error should be gone!

## Need Help?

If you're still having trouble:
1. Check `QUICK_FIREBASE_SETUP.md` for more details
2. Verify you have access to the Firebase project
3. Make sure the JSON file is valid (try opening it in a text editor)

