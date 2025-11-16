#!/bin/bash
# filepath: /Users/ronald/repos/github.CnP/PXL-Clock/build/start-all.sh
# Start Simulator and F# Watcher concurrently

# Start the Simulator in the background
./build/start-simulator.sh &

# Start the F# Watcher in the background
./build/start-fsharp-watcher.sh &

# Wait for both processes to finish (if they ever do)
wait
