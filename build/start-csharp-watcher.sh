#!/bin/bash
set -e

echo "Starting C# watcher ..."
dotnet fsi ./build/csWatcher.fsx
