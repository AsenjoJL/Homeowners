# Quick Steps to Create Admin Account

## Option 1: Use the Tool (Recommended)

Run this command:
```powershell
dotnet run --project Tools/CreateAdminTool.csproj
```

This will generate all the data you need. Then follow the steps below.

## Option 2: Manual Creation

### Step 1: Go to Firebase Console
https://console.firebase.google.com/project/homeowner-c355d/firestore

### Step 2: Create Collection
1. Click **"Start collection"**
2. Collection ID: `admins`
3. Click **"Next"**

### Step 3: Add Document
1. Document ID: `1`
2. Add these 7 fields:

| Field | Type | Value |
|-------|------|-------|
| AdminID | number | 1 |
| FullName | string | System Administrator |
| Email | string | admin@homeowner.com |
| PasswordHash | string | `0enXmdjDXmKslZZuGZ9Lhw==:1q17tkKZ1PWdZQJfYtDmuon+Iy0oThz8VaLhRHNriCM=` |
| Role | string | Admin |
| OfficeLocation | string | Main Office |
| Status | string | Active |

3. Click **"Save"**

### Step 4: Test Login
1. Set Firebase credentials: `$env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\key.json"`
2. Run: `dotnet run`
3. Login at: http://localhost:5020/Account/Login
4. Email: `admin@homeowner.com`
5. Password: `Admin123!`

## Done! ✅

You can now log in as admin and start using the system.

