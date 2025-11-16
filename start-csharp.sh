#!/bin/bash
# filepath: /Users/ronald/repos/github.CnP/PXL-Clock/start-csharp.sh
# Start Simulator and C# Watcher concurrently

# Start the Simulator in the background
./build/start-simulator.sh &

# Start the C# Watcher in the background
./build/start-csharp-watcher.sh &

# Wait for both processes to finish (if they ever do)
wait
