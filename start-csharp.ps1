# Start Simulator and C# Watcher concurrently

$simJob = Start-Job -ScriptBlock { & "./build/start-simulator.ps1" }
$watchJob = Start-Job -ScriptBlock { & "./build/start-csharp-watcher.ps1" }

Start-Process "http://127.0.0.1:5001"

# Wait for both jobs to complete
Wait-Job -Job $simJob, $watchJob

# Optionally output results or cleanup jobs
Receive-Job -Job $simJob, $watchJob | Write-Output
Remove-Job -Job $simJob, $watchJob
