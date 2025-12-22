#!/bin/bash
# filepath: /Users/ronald/repos/github.CnP/PXL-Clock/start.sh
# Start Simulator and C# Watcher concurrently

# Start the Simulator in the background
./build/start-simulator.sh &

# Start the C# Watcher in the background
./build/start-watcher.sh &

open http://127.0.0.1:5001 &

# Wait for both processes to finish (if they ever do)
wait
