using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

Console.WriteLine("=== Admin Account Creator ===\n");

// Configuration
var adminEmail = "admin@homeowner.com";
var adminPassword = "Admin123!"; // ⚠️ CHANGE THIS PASSWORD!
var adminName = "System Administrator";

// Generate password hash
Console.WriteLine("Generating password hash...");
var saltBytes = RandomNumberGenerator.GetBytes(16);
var hashBytes = KeyDerivation.Pbkdf2(
    password: adminPassword,
    salt: saltBytes,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: 100000,
    numBytesRequested: 32);

var passwordHash = $"{Convert.ToBase64String(saltBytes)}:{Convert.ToBase64String(hashBytes)}";

// Display results
Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("ADMIN ACCOUNT DATA FOR FIREBASE FIRESTORE");
Console.WriteLine(new string('=', 60) + "\n");

Console.WriteLine("Collection: admins");
Console.WriteLine("Document ID: 1\n");

Console.WriteLine("Fields to add in Firebase Console:\n");
Console.WriteLine($"  AdminID (number):         1");
Console.WriteLine($"  FullName (string):        {adminName}");
Console.WriteLine($"  Email (string):           {adminEmail}");
Console.WriteLine($"  PasswordHash (string):    {passwordHash}");
Console.WriteLine($"  Role (string):            Admin");
Console.WriteLine($"  OfficeLocation (string):  Main Office");
Console.WriteLine($"  Status (string):          Active");

Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("JSON FORMAT (Copy this entire block):");
Console.WriteLine(new string('=', 60) + "\n");

var json = $@"{{
  ""AdminID"": 1,
  ""FullName"": ""{adminName}"",
  ""Email"": ""{adminEmail}"",
  ""PasswordHash"": ""{passwordHash}"",
  ""Role"": ""Admin"",
  ""OfficeLocation"": ""Main Office"",
  ""Status"": ""Active""
}}";

Console.WriteLine(json);

Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("LOGIN CREDENTIALS:");
Console.WriteLine(new string('=', 60));
Console.WriteLine($"Email:    {adminEmail}");
Console.WriteLine($"Password: {adminPassword}");
Console.WriteLine("\n⚠️  IMPORTANT: Change the password after first login!\n");

Console.WriteLine(new string('=', 60));
Console.WriteLine("STEPS TO ADD TO FIREBASE:");
Console.WriteLine(new string('=', 60));
Console.WriteLine("1. Go to: https://console.firebase.google.com/project/homeowner-c355d/firestore");
Console.WriteLine("2. Click 'Start collection' (if this is your first collection)");
Console.WriteLine("3. Collection ID: admins");
Console.WriteLine("4. Click 'Next'");
Console.WriteLine("5. Document ID: 1");
Console.WriteLine("6. Add fields one by one:");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: AdminID, Type: number, Value: 1");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: FullName, Type: string, Value: System Administrator");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: Email, Type: string, Value: admin@homeowner.com");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: PasswordHash, Type: string, Value: [paste the hash above]");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: Role, Type: string, Value: Admin");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: OfficeLocation, Type: string, Value: Main Office");
Console.WriteLine("   - Click 'Add field'");
Console.WriteLine("   - Field name: Status, Type: string, Value: Active");
Console.WriteLine("7. Click 'Save'");
Console.WriteLine("\n" + new string('=', 60));
