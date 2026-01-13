# PowerShell script to set up Firebase credentials
# Run this script before starting the application

Write-Host "=== Firebase Setup Script ===" -ForegroundColor Cyan
Write-Host ""

# Check if service account file exists
$serviceAccountPath = Read-Host "Enter the full path to your Firebase service account JSON file (e.g., C:\firebase-keys\homeowner-c355d-firebase-adminsdk-xxxxx.json)"

if (-not (Test-Path $serviceAccountPath)) {
    Write-Host "ERROR: File not found at: $serviceAccountPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "To get your service account key:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://console.firebase.google.com/project/homeowner-c355d/settings/serviceaccounts/adminsdk" -ForegroundColor Yellow
    Write-Host "2. Click 'Generate new private key'" -ForegroundColor Yellow
    Write-Host "3. Download the JSON file" -ForegroundColor Yellow
    Write-Host "4. Run this script again with the correct path" -ForegroundColor Yellow
    exit 1
}

# Set environment variable for current session
$env:GOOGLE_APPLICATION_CREDENTIALS = $serviceAccountPath

Write-Host ""
Write-Host "✓ Environment variable set for current session" -ForegroundColor Green
Write-Host "  GOOGLE_APPLICATION_CREDENTIALS = $serviceAccountPath" -ForegroundColor Gray
Write-Host ""

# Optionally set it permanently
$setPermanent = Read-Host "Do you want to set this permanently for your user? (Y/N)"
if ($setPermanent -eq "Y" -or $setPermanent -eq "y") {
    [System.Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $serviceAccountPath, "User")
    Write-Host "✓ Environment variable set permanently for your user" -ForegroundColor Green
    Write-Host "  (You may need to restart your terminal/IDE for it to take effect)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Cyan
Write-Host "You can now run: dotnet run" -ForegroundColor Green
Write-Host ""

