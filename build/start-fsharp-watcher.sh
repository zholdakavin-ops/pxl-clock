#!/bin/bash
set -e

echo "Starting F# watcher ..."
dotnet fsi ./build/fsxWatcher.fsx
