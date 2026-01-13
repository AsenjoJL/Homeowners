# Script to help find and set Firebase service account key

Write-Host "=== Firebase Service Account Key Finder ===" -ForegroundColor Cyan
Write-Host ""

# Check if key already exists in Downloads
$downloadsPath = "$env:USERPROFILE\Downloads"
$firebaseKeyFiles = Get-ChildItem -Path $downloadsPath -Filter "*firebase-adminsdk*.json" -ErrorAction SilentlyContinue

if ($firebaseKeyFiles) {
    Write-Host "Found Firebase key file(s) in Downloads:" -ForegroundColor Green
    Write-Host ""
    $index = 1
    foreach ($file in $firebaseKeyFiles) {
        Write-Host "$index. $($file.FullName)" -ForegroundColor White
        Write-Host "   Size: $([math]::Round($file.Length / 1KB, 2)) KB" -ForegroundColor Gray
        Write-Host "   Modified: $($file.LastWriteTime)" -ForegroundColor Gray
        Write-Host ""
        $index++
    }
    
    if ($firebaseKeyFiles.Count -eq 1) {
        $selectedFile = $firebaseKeyFiles[0]
        Write-Host "Using the found file: $($selectedFile.FullName)" -ForegroundColor Yellow
        Write-Host ""
        
        # Ask to move to a better location
        $moveTo = Read-Host "Move to C:\firebase-keys\? (Y/N)"
        if ($moveTo -eq "Y" -or $moveTo -eq "y") {
            $targetDir = "C:\firebase-keys"
            if (-not (Test-Path $targetDir)) {
                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
            }
            $targetPath = Join-Path $targetDir $selectedFile.Name
            Move-Item -Path $selectedFile.FullName -Destination $targetPath -Force
            Write-Host "Moved to: $targetPath" -ForegroundColor Green
            $selectedFile = Get-Item $targetPath
        }
        
        # Set environment variable
        $env:GOOGLE_APPLICATION_CREDENTIALS = $selectedFile.FullName
        Write-Host ""
        Write-Host "✓ Environment variable set for current session" -ForegroundColor Green
        Write-Host "  GOOGLE_APPLICATION_CREDENTIALS = $($selectedFile.FullName)" -ForegroundColor Gray
        
        # Ask to set permanently
        $setPermanent = Read-Host "`nSet permanently for your user? (Y/N)"
        if ($setPermanent -eq "Y" -or $setPermanent -eq "y") {
            [System.Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $selectedFile.FullName, "User")
            Write-Host "✓ Set permanently (restart terminal/IDE for it to take effect)" -ForegroundColor Green
        }
    } else {
        Write-Host "Multiple files found. Please select one:" -ForegroundColor Yellow
        $choice = Read-Host "Enter number (1-$($firebaseKeyFiles.Count))"
        $selectedIndex = [int]$choice - 1
        if ($selectedIndex -ge 0 -and $selectedIndex -lt $firebaseKeyFiles.Count) {
            $selectedFile = $firebaseKeyFiles[$selectedIndex]
            $env:GOOGLE_APPLICATION_CREDENTIALS = $selectedFile.FullName
            Write-Host "✓ Set to: $($selectedFile.FullName)" -ForegroundColor Green
        }
    }
} else {
    Write-Host "No Firebase key file found in Downloads folder." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To get your Firebase service account key:" -ForegroundColor Cyan
    Write-Host "1. Go to: https://console.firebase.google.com/project/homeowner-c355d/settings/serviceaccounts/adminsdk" -ForegroundColor White
    Write-Host "2. Click 'Generate new private key'" -ForegroundColor White
    Write-Host "3. Click 'Generate key' in the popup" -ForegroundColor White
    Write-Host "4. The JSON file will download to your Downloads folder" -ForegroundColor White
    Write-Host "5. Run this script again to set it up automatically" -ForegroundColor White
    Write-Host ""
    
    $manualPath = Read-Host "Or enter the full path to your Firebase key file (if you have it elsewhere)"
    if ($manualPath -and (Test-Path $manualPath)) {
        $env:GOOGLE_APPLICATION_CREDENTIALS = $manualPath
        Write-Host "✓ Set to: $manualPath" -ForegroundColor Green
    } else {
        Write-Host "File not found. Please download it first." -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "=== Verification ===" -ForegroundColor Cyan
Write-Host "Environment variable: $env:GOOGLE_APPLICATION_CREDENTIALS" -ForegroundColor White
if (Test-Path $env:GOOGLE_APPLICATION_CREDENTIALS) {
    Write-Host "✓ File exists" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run: dotnet run" -ForegroundColor Green
} else {
    Write-Host "✗ File not found at that path" -ForegroundColor Red
}

Write-Host ""

