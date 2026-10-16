#!/bin/bash
# MSTest создаёт настоящие протоколы HTML и TRX рядом с отчётом.
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
export NUGET_PACKAGES="$semester/.dotnet/packages"
"$sdk" test "$folder/код/CollectionsPractice.Tests/CollectionsPractice.Tests.csproj" \
    -c Release --disable-build-servers -p:UseSharedCompilation=false \
    --logger "console;verbosity=normal" --logger "trx;LogFileName=tests.trx" \
    --logger "html;LogFileName=tests.html" --results-directory "$folder/результаты тестов" "$@"
status=$?
if [ -t 0 ]; then read -r -p 'Нажмите Enter, чтобы закрыть окно… ' _; fi
exit "$status"
