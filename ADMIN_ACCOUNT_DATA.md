# Admin Account Data - Ready to Add to Firebase

## Generated Admin Account

**Collection:** `admins`  
**Document ID:** `1`

## Fields to Add in Firebase Console

| Field Name | Type | Value |
|------------|------|-------|
| AdminID | number | 1 |
| FullName | string | System Administrator |
| Email | string | admin@homeowner.com |
| PasswordHash | string | `0enXmdjDXmKslZZuGZ9Lhw==:1q17tkKZ1PWdZQJfYtDmuon+Iy0oThz8VaLhRHNriCM=` |
| Role | string | Admin |
| OfficeLocation | string | Main Office |
| Status | string | Active |

## JSON Format (Copy This)

```json
{
  "AdminID": 1,
  "FullName": "System Administrator",
  "Email": "admin@homeowner.com",
  "PasswordHash": "0enXmdjDXmKslZZuGZ9Lhw==:1q17tkKZ1PWdZQJfYtDmuon+Iy0oThz8VaLhRHNriCM=",
  "Role": "Admin",
  "OfficeLocation": "Main Office",
  "Status": "Active"
}
```

## Login Credentials

- **Email:** `admin@homeowner.com`
- **Password:** `Admin123!`

⚠️ **IMPORTANT:** Change the password after first login!

## Steps to Add to Firebase

1. **Go to Firebase Console:**
   https://console.firebase.google.com/project/homeowner-c355d/firestore

2. **Create Collection:**
   - Click **"Start collection"** (if this is your first collection)
   - Collection ID: `admins`
   - Click **"Next"**

3. **Add Document:**
   - Document ID: `1`
   - Click **"Add field"** for each field below:

   **Field 1:**
   - Field name: `AdminID`
   - Type: `number`
   - Value: `1`

   **Field 2:**
   - Field name: `FullName`
   - Type: `string`
   - Value: `System Administrator`

   **Field 3:**
   - Field name: `Email`
   - Type: `string`
   - Value: `admin@homeowner.com`

   **Field 4:**
   - Field name: `PasswordHash`
   - Type: `string`
   - Value: `0enXmdjDXmKslZZuGZ9Lhw==:1q17tkKZ1PWdZQJfYtDmuon+Iy0oThz8VaLhRHNriCM=`

   **Field 5:**
   - Field name: `Role`
   - Type: `string`
   - Value: `Admin`

   **Field 6:**
   - Field name: `OfficeLocation`
   - Type: `string`
   - Value: `Main Office`

   **Field 7:**
   - Field name: `Status`
   - Type: `string`
   - Value: `Active`

4. **Save:**
   - Click **"Save"**

## Verify

After adding, you should see:
- Collection: `admins`
- Document: `1`
- 7 fields total

## Test Login

After adding the admin account:

1. Make sure Firebase credentials are set:
   ```powershell
   $env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\your-firebase-key.json"
   ```

2. Run the application:
   ```powershell
   dotnet run
   ```

3. Go to: http://localhost:5020/Account/Login

4. Login with:
   - Email: `admin@homeowner.com`
   - Password: `Admin123!`

5. You should be redirected to the Admin Dashboard!

## Regenerate Admin Account

To create a new admin account with different credentials, run:

```powershell
dotnet run --project Tools/CreateAdminTool.csproj
```

This will generate new password hash for your specified credentials.

