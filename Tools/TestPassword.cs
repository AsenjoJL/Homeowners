using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

// Auto-detect Firebase credentials if not set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
{
    var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    var firebaseKeyFile = Directory.GetFiles(downloadsPath, "*homeowner-c355d-firebase*.json")
        .FirstOrDefault();
    
    if (firebaseKeyFile != null && File.Exists(firebaseKeyFile))
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseKeyFile);
    }
}

try
{
    var db = FirestoreDb.Create("homeowner-c355d");
    var adminDoc = await db.Collection("admins").Document("1").GetSnapshotAsync();
    
    if (!adminDoc.Exists)
    {
        Console.WriteLine("✗ Admin not found!");
        return;
    }
    
    var adminData = adminDoc.ToDictionary();
    var storedHash = adminData["PasswordHash"]?.ToString() ?? "";
    var testPassword = "Admin123!";
    
    Console.WriteLine("=== Testing Password Hash ===\n");
    Console.WriteLine($"Stored hash: {storedHash.Substring(0, Math.Min(30, storedHash.Length))}...\n");
    
    // Verify password
    if (string.IsNullOrWhiteSpace(storedHash))
    {
        Console.WriteLine("✗ Password hash is empty!");
        return;
    }
    
    string[] parts = storedHash.Split(':');
    if (parts.Length != 2)
    {
        Console.WriteLine("✗ Password hash format is incorrect!");
        return;
    }
    
    byte[] salt, storedHashBytes;
    try
    {
        salt = Convert.FromBase64String(parts[0]);
        storedHashBytes = Convert.FromBase64String(parts[1]);
    }
    catch (FormatException)
    {
        Console.WriteLine("✗ Password hash format is invalid!");
        return;
    }
    
    byte[] enteredHashBytes = KeyDerivation.Pbkdf2(
        password: testPassword,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 100000,
        numBytesRequested: 32
    );
    
    bool passwordMatches = enteredHashBytes.SequenceEqual(storedHashBytes);
    
    if (passwordMatches)
    {
        Console.WriteLine("✓ Password hash is CORRECT!");
        Console.WriteLine($"✓ Password '{testPassword}' matches the stored hash\n");
    }
    else
    {
        Console.WriteLine("✗ Password hash does NOT match!");
        Console.WriteLine($"✗ Password '{testPassword}' does not match the stored hash\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
}

