# Script to set Firebase credentials and start the application

Write-Host "=== Starting HOMEOWNER Application ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Set Firebase credentials
Write-Host "Step 1: Setting Firebase credentials..." -ForegroundColor Yellow
$firebaseKey = Get-ChildItem -Path "$env:USERPROFILE\Downloads" -Filter "*homeowner-c355d-firebase*.json" | Select-Object -First 1

if ($firebaseKey) {
    $env:GOOGLE_APPLICATION_CREDENTIALS = $firebaseKey.FullName
    Write-Host "✓ Credentials set: $($firebaseKey.FullName)" -ForegroundColor Green
} else {
    Write-Host "✗ Firebase key file not found in Downloads" -ForegroundColor Red
    Write-Host "  Looking for: *homeowner-c355d-firebase*.json" -ForegroundColor Gray
    Write-Host ""
    $manualPath = Read-Host "Enter the full path to your Firebase key file"
    if (Test-Path $manualPath) {
        $env:GOOGLE_APPLICATION_CREDENTIALS = $manualPath
        Write-Host "✓ Credentials set" -ForegroundColor Green
    } else {
        Write-Host "✗ File not found. Exiting." -ForegroundColor Red
        exit 1
    }
}

# Step 2: Stop any running instances
Write-Host "`nStep 2: Stopping any running instances..." -ForegroundColor Yellow
Get-Process -Name "HOMEOWNER" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

# Check for port 5020
$portInUse = Get-NetTCPConnection -LocalPort 5020 -ErrorAction SilentlyContinue
if ($portInUse) {
    $pid = $portInUse.OwningProcess
    Write-Host "  Stopping process using port 5020 (PID: $pid)" -ForegroundColor Yellow
    Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

Write-Host "✓ Ready to start" -ForegroundColor Green

# Step 3: Start the application
Write-Host "`nStep 3: Starting application..." -ForegroundColor Yellow
Write-Host "  Access at: http://localhost:5020" -ForegroundColor Cyan
Write-Host "  Press Ctrl+C to stop" -ForegroundColor Gray
Write-Host ""

dotnet run

