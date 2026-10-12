#!/bin/bash
folder="$(cd "$(dirname "$0")" && pwd)"
exec /bin/bash "$folder/код/ComputerEquipmentLab.Tests/run.command" "$@"
