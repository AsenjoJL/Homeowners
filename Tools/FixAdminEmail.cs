using Google.Cloud.Firestore;

// Auto-detect Firebase credentials if not set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
{
    var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    var firebaseKeyFile = Directory.GetFiles(downloadsPath, "*homeowner-c355d-firebase*.json")
        .FirstOrDefault();
    
    if (firebaseKeyFile != null && File.Exists(firebaseKeyFile))
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseKeyFile);
        Console.WriteLine($"✓ Using Firebase credentials: {firebaseKeyFile}");
    }
    else
    {
        Console.WriteLine("✗ Firebase credentials not found!");
        return;
    }
}

try
{
    var db = FirestoreDb.Create("homeowner-c355d");
    var adminsCollection = db.Collection("admins");
    
    Console.WriteLine("\n=== Fixing Admin Email Field ===\n");
    
    // Get admin by document ID
    var adminDoc = await adminsCollection.Document("1").GetSnapshotAsync();
    
    if (!adminDoc.Exists)
    {
        Console.WriteLine("✗ Admin document with ID '1' not found!");
        return;
    }
    
    var adminData = adminDoc.ToDictionary();
    Console.WriteLine("Current Email value:");
    Console.WriteLine($"  '{adminData["Email"]}'");
    Console.WriteLine($"  Length: {adminData["Email"]?.ToString()?.Length ?? 0}");
    Console.WriteLine($"  Has whitespace: {!string.IsNullOrWhiteSpace(adminData["Email"]?.ToString()) && adminData["Email"]?.ToString() != adminData["Email"]?.ToString()?.Trim()}");
    
    // Fix email field (trim whitespace)
    var currentEmail = adminData["Email"]?.ToString() ?? "";
    var cleanEmail = currentEmail.Trim();
    
    if (currentEmail != cleanEmail)
    {
        Console.WriteLine($"\nFixing email: '{currentEmail}' -> '{cleanEmail}'");
        
        var updateData = new Dictionary<string, object>
        {
            { "Email", cleanEmail }
        };
        
        await adminDoc.Reference.UpdateAsync(updateData);
        Console.WriteLine("✓ Email field fixed!");
    }
    else
    {
        Console.WriteLine("\n✓ Email field is already clean (no whitespace)");
    }
    
    // Verify the fix
    Console.WriteLine("\nVerifying...");
    var query = adminsCollection.WhereEqualTo("Email", "admin@homeowner.com");
    var snapshot = await query.GetSnapshotAsync();
    
    if (snapshot.Count > 0)
    {
        Console.WriteLine("✓ Query by email now works!");
    }
    else
    {
        Console.WriteLine("✗ Query still doesn't work. Checking case sensitivity...");
        
        // Try case-insensitive search
        var allAdmins = await adminsCollection.GetSnapshotAsync();
        foreach (var doc in allAdmins.Documents)
        {
            var data = doc.ToDictionary();
            var email = data["Email"]?.ToString() ?? "";
            if (email.Equals("admin@homeowner.com", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Found email with different case: '{email}'");
                // Update to correct case
                await doc.Reference.UpdateAsync(new Dictionary<string, object> { { "Email", "admin@homeowner.com" } });
                Console.WriteLine("✓ Fixed email case");
            }
        }
    }
    
    Console.WriteLine("\n✓ Done! Try logging in again.");
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ Error: {ex.Message}");
    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
}

