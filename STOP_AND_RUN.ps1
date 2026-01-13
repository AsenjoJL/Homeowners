# Script to stop processes using port 5020 and run the application

Write-Host "=== Stopping processes on port 5020 ===" -ForegroundColor Cyan

# Find processes using port 5020
$connections = Get-NetTCPConnection -LocalPort 5020 -ErrorAction SilentlyContinue
if ($connections) {
    foreach ($conn in $connections) {
        $processId = $conn.OwningProcess
        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "Found process: $($process.ProcessName) (PID: $processId)" -ForegroundColor Yellow
            Write-Host "Stopping process..." -ForegroundColor Cyan
            Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
            Start-Sleep -Seconds 1
            Write-Host "✓ Stopped" -ForegroundColor Green
        }
    }
} else {
    Write-Host "Port 5020 is free" -ForegroundColor Green
}

# Also stop any HOMEOWNER processes
Write-Host "`n=== Stopping HOMEOWNER processes ===" -ForegroundColor Cyan
Get-Process -Name "HOMEOWNER" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "Stopping HOMEOWNER (PID: $($_.Id))" -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}
Start-Sleep -Seconds 2

Write-Host "`n=== Starting application ===" -ForegroundColor Cyan
Write-Host "Running: dotnet run" -ForegroundColor White
Write-Host ""

dotnet run

