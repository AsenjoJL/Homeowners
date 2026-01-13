# Fix Port Already in Use Error

## Quick Fix

If you get "address already in use" error, run:

```powershell
.\STOP_AND_RUN.ps1
```

This script will:
1. Stop any process using port 5020
2. Stop any running HOMEOWNER processes
3. Start the application fresh

## Manual Fix

### Option 1: Stop the Process Using Port 5020

```powershell
# Find what's using port 5020
Get-NetTCPConnection -LocalPort 5020 | Select-Object OwningProcess

# Stop it (replace PID with actual process ID)
Stop-Process -Id <PID> -Force

# Then run
dotnet run
```

### Option 2: Change the Port

Edit `Properties/launchSettings.json` and change port 5020 to something else like 5021 or 8080.

### Option 3: Kill All HOMEOWNER Processes

```powershell
Get-Process -Name "HOMEOWNER" -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet run
```

## Why This Happens

- The application is already running in another terminal
- A previous instance didn't shut down properly
- Another application is using port 5020

## Prevention

Always stop the application properly:
- Press **Ctrl+C** in the terminal where it's running
- Or close the terminal window before running again

