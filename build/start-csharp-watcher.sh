#!/bin/bash
set -e

echo "Starting C#/F# watcher ..."
dotnet fsi ./build/csFsxWatcher.fsx
