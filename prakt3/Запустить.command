#!/bin/bash
# Одна программа в одном окне; .NET берётся из папки второго семестра.
set -u
folder="$(cd "$(dirname "$0")" && pwd)"
semester="$(cd "$folder/.." && pwd)"
sdk="$semester/.dotnet/dotnet"
if [ ! -x "$sdk" ]; then sdk="$(command -v dotnet || true)"; fi
if [ -z "$sdk" ] || [ ! -x "$sdk" ]; then
    printf 'Не найден .NET SDK 8.\n'
    read -r -p 'Нажмите Enter… ' _
    exit 1
fi
export DOTNET_CLI_HOME="$semester/.dotnet/cli"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
"$sdk" run --project "$folder/код/CollectionsPractice/CollectionsPractice.csproj" \
    -c Release --disable-build-servers -p:UseSharedCompilation=false -- "$@"
status=$?
if [ "$status" -ne 0 ] && [ -t 0 ]; then read -r -p 'Нажмите Enter… ' _; fi
exit "$status"
