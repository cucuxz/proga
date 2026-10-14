#!/bin/bash
project_dir="$(cd "$(dirname "$0")" && pwd)"
if [ "$(uname -s)" = "Darwin" ]; then
    # Отдельное окно с палитрой Norton Commander, без смены стандартного профиля.
    exec /usr/bin/open "$project_dir/Запустить.terminal"
fi
exec /bin/bash "$project_dir/код/NortonCommanderConsole/run.command" "$@"
