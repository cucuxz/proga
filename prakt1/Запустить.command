#!/bin/bash
# Открывается двойным щелчком в Finder.
folder="$(cd "$(dirname "$0")" && pwd)"
exec /bin/bash "$folder/код/NortonCommanderConsole/run.command" "$@"
