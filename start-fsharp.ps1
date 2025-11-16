# Start Simulator and F# Watcher concurrently

$simJob = Start-Job -ScriptBlock { & "./start-simulator.ps1" }
$watchJob = Start-Job -ScriptBlock { & "./start-fsharp-watcher.ps1" }

# Wait for both jobs to complete
Wait-Job -Job $simJob, $watchJob

# Optionally output results or cleanup jobs
Receive-Job -Job $simJob, $watchJob | Write-Output
Remove-Job -Job $simJob, $watchJob
