#!/bin/bash
project_dir="$(cd "$(dirname "$0")" && pwd)"
# Запускаем программу в этом же окне, без открытия второго Terminal.
exec /bin/bash "$project_dir/код/NortonCommanderConsole/run.command" "$@"
