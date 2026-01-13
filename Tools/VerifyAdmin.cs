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
    
    Console.WriteLine("\n=== Checking Admin Account ===");
    Console.WriteLine("Email: admin@homeowner.com\n");
    
    // Check by email
    var query = adminsCollection.WhereEqualTo("Email", "admin@homeowner.com");
    var snapshot = await query.GetSnapshotAsync();
    
    if (snapshot.Count == 0)
    {
        Console.WriteLine("✗ Admin account NOT FOUND in Firebase!");
        Console.WriteLine("\nChecking all admins in collection...\n");
        
        var allAdmins = await adminsCollection.GetSnapshotAsync();
        if (allAdmins.Count == 0)
        {
            Console.WriteLine("✗ No admins found in 'admins' collection!");
            Console.WriteLine("\nYou need to add the admin account to Firebase Firestore.");
            Console.WriteLine("See ADMIN_ACCOUNT_DATA.md for instructions.\n");
        }
        else
        {
            Console.WriteLine($"Found {allAdmins.Count} admin(s):\n");
            foreach (var doc in allAdmins.Documents)
            {
                var data = doc.ToDictionary();
                Console.WriteLine($"Document ID: {doc.Id}");
                foreach (var field in data)
                {
                    Console.WriteLine($"  {field.Key}: {field.Value}");
                }
                Console.WriteLine();
            }
        }
    }
    else
    {
        Console.WriteLine("✓ Admin account FOUND!\n");
        var adminDoc = snapshot.Documents.First();
        var adminData = adminDoc.ToDictionary();
        
        Console.WriteLine("Admin Details:");
        Console.WriteLine($"  Document ID: {adminDoc.Id}");
        foreach (var field in adminData)
        {
            if (field.Key == "PasswordHash")
            {
                var hash = field.Value?.ToString() ?? "";
                Console.WriteLine($"  {field.Key}: {hash.Substring(0, Math.Min(20, hash.Length))}...");
            }
            else
            {
                Console.WriteLine($"  {field.Key}: {field.Value}");
            }
        }
        
        // Verify password hash format
        if (adminData.ContainsKey("PasswordHash"))
        {
            var hash = adminData["PasswordHash"]?.ToString() ?? "";
            if (hash.Contains(":"))
            {
                Console.WriteLine("\n✓ Password hash format is correct (salt:hash)");
            }
            else
            {
                Console.WriteLine("\n✗ Password hash format is incorrect! Should be 'salt:hash'");
            }
        }
        else
        {
            Console.WriteLine("\n✗ PasswordHash field is missing!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ Error: {ex.Message}");
    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
}

