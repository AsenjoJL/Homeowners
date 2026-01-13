# PowerShell script to create admin account and add to Firebase
# This script generates the password hash and provides instructions

Write-Host "=== Admin Account Creator ===" -ForegroundColor Cyan
Write-Host ""

# Configuration
$adminEmail = Read-Host "Enter admin email (default: admin@homeowner.com)"
if ([string]::IsNullOrWhiteSpace($adminEmail)) {
    $adminEmail = "admin@homeowner.com"
}

$adminPassword = Read-Host "Enter admin password (default: Admin123!)" -AsSecureString
$adminPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($adminPassword))
if ([string]::IsNullOrWhiteSpace($adminPasswordPlain)) {
    $adminPasswordPlain = "Admin123!"
}

$adminName = Read-Host "Enter admin full name (default: System Administrator)"
if ([string]::IsNullOrWhiteSpace($adminName)) {
    $adminName = "System Administrator"
}

Write-Host ""
Write-Host "Generating password hash..." -ForegroundColor Yellow

# Note: This is a simplified version. For production, use the C# tool
Write-Host ""
Write-Host "=== Admin Account Data ===" -ForegroundColor Green
Write-Host ""
Write-Host "Collection: admins"
Write-Host "Document ID: 1"
Write-Host ""
Write-Host "Fields to add:" -ForegroundColor Yellow
Write-Host "  AdminID: 1 (number)"
Write-Host "  FullName: $adminName (string)"
Write-Host "  Email: $adminEmail (string)"
Write-Host "  PasswordHash: [GENERATED BELOW] (string)"
Write-Host "  Role: Admin (string)"
Write-Host "  OfficeLocation: Main Office (string)"
Write-Host "  Status: Active (string)"
Write-Host ""

Write-Host "=== IMPORTANT ===" -ForegroundColor Red
Write-Host "To generate the password hash, you need to run the C# tool:" -ForegroundColor Yellow
Write-Host "  dotnet run --project CreateAdminTool" -ForegroundColor White
Write-Host ""
Write-Host "Or manually add the account using Firebase Console:" -ForegroundColor Yellow
Write-Host "1. Go to: https://console.firebase.google.com/project/homeowner-c355d/firestore" -ForegroundColor White
Write-Host "2. Create collection 'admins'" -ForegroundColor White
Write-Host "3. Add document with ID '1'" -ForegroundColor White
Write-Host "4. Add the fields listed above" -ForegroundColor White
Write-Host ""
Write-Host "For password hash generation, see CREATE_ADMIN_SCRIPT.txt" -ForegroundColor Yellow
Write-Host ""

