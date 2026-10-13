#!/bin/bash
# Запуск из папки проекта: использует .NET из папки второго семестра.
set -u
project_dir="$(cd "$(dirname "$0")" && pwd)"
semester_dir="$(cd "$project_dir/../../.." && pwd)"
dotnet_exe="$semester_dir/.dotnet/dotnet"
if [ ! -x "$dotnet_exe" ]; then
    dotnet_exe="$(command -v dotnet || true)"
fi
if [ -z "$dotnet_exe" ] || [ ! -x "$dotnet_exe" ]; then
    printf 'Не найден .NET SDK. Нужна папка .dotnet в proga 2 sem или установленный .NET SDK 8.\n'
    read -r -p 'Нажмите Enter, чтобы закрыть окно… ' _
    exit 1
fi
export DOTNET_CLI_HOME="$semester_dir/.dotnet/cli"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export NUGET_PACKAGES="$semester_dir/.dotnet/packages"
export DOTNET_GENERATE_ASPNET_CERTIFICATE=false
cd "$project_dir" || exit 1
"$dotnet_exe" test "$project_dir/ComputerEquipmentLab.Tests.csproj" --configuration Release --disable-build-servers -p:UseSharedCompilation=false --logger "console;verbosity=normal" --logger "trx;LogFileName=tests.trx" --logger "html;LogFileName=tests.html" --results-directory "$project_dir/../../результаты тестов" "$@"
result=$?
if [ "$result" -ne 0 ]; then
    printf '\nПрограмма завершилась с ошибкой. Проверьте сообщение выше.\n'
fi
if [ -t 0 ]; then
    read -r -p "Нажмите Enter, чтобы закрыть окно… " _
fi
exit "$result"
