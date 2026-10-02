using System.Globalization;
using System.Text;

namespace NortonCommanderConsole;

// 1. Program запускает программу и хранит демонстрационный список.
public static class Program
{
    public static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        List<FileEntry> files = CreateFiles();
        Commander commander = new Commander();
        if (Console.IsOutputRedirected)
        {
            commander.Draw(files);
            return 0;
        }
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Console.SetBufferSize(Math.Max(Console.BufferWidth, 80),
                    Math.Max(Console.BufferHeight, 25));
                Console.SetWindowSize(Commander.Width, Commander.Height);
                Console.SetBufferSize(Commander.Width, Commander.Height);
            }
            if (Console.WindowWidth < 80 || Console.WindowHeight < 25)
            {
                Console.Error.WriteLine("Увеличьте терминал минимум до 80 столбцов и 25 строк.");
                return 1;
            }
            commander.Draw(files);
            if (!Console.IsInputRedirected) WaitForKey(commander, files);
        }
        catch (Exception error) when (error is IOException or
            ArgumentOutOfRangeException or PlatformNotSupportedException)
        {
            Console.Error.WriteLine("Ошибка консоли: " + error.Message);
            return 1;
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
        return 0;
    }

    private static void WaitForKey(Commander commander, List<FileEntry> files)
    {
        int oldWidth = Console.WindowWidth, oldHeight = Console.WindowHeight;
        // Перерисовываем панели только при изменении размеров терминала.
        while (!Console.KeyAvailable)
        {
            int width = Console.WindowWidth, height = Console.WindowHeight;
            if (width != oldWidth || height != oldHeight)
            {
                oldWidth = width;
                oldHeight = height;
                if (width >= 80 && height >= 25) commander.Draw(files);
                else
                {
                    Console.ResetColor();
                    Console.Clear();
                    Console.Write("Увеличьте терминал до 80×25.");
                }
            }
            Thread.Sleep(100);
        }
        Console.ReadKey(true);
    }

    public static List<FileEntry> CreateFiles()
    {
        DateTime oldDate = new DateTime(1995, 5, 25, 5, 0, 0);
        DateTime newDate = new DateTime(2002, 10, 11, 19, 48, 0);
        // Все записи находятся здесь. Чтобы добавить файл, добавьте одну строку:
        // new FileEntry("Имя.txt", размерВБайтах, датаИзменения).
        // Последний аргумент true обозначает каталог.
        List<FileEntry> files = new List<FileEntry>
        {
            new FileEntry("DOCS", 0, newDate, true),
            new FileEntry("HELP", 0, newDate, true),
            new FileEntry("TEMP", 0, newDate, true),
            new FileEntry("123view.exe", 128380, oldDate),
            new FileEntry("AjaccgdWithLongName.exe", 417392, newDate),
            new FileEntry("arcview.exe", 81738, oldDate),
            new FileEntry("bitmap.exe", 54805, oldDate),
            new FileEntry("bug.nss", 16133, oldDate),
            new FileEntry("bungee.nss", 41914, oldDate),
            new FileEntry("MyFileWithLongName.txt", 2048, newDate),
            new FileEntry("VeryLongDirectoryName", 0, newDate, true),
            new FileEntry("4372ansi.set", 255, oldDate),
            new FileEntry("8502ansi.set", 1279, oldDate),
            new FileEntry("8632ansi.set", 2303, oldDate),
            new FileEntry("8652ansi.set", 3327, oldDate),
            new FileEntry("8662ansi.set", 4351, oldDate),
            new FileEntry("ansi2437.set", 5375, oldDate),
            new FileEntry("ansi2850.set", 6399, oldDate),
            new FileEntry("ansi2863.set", 7423, oldDate),
            new FileEntry("ansi2865.set", 8447, oldDate),
            new FileEntry("ansi2866.set", 9471, oldDate),
            new FileEntry("clp2dib.exe", 10495, oldDate),
            new FileEntry("dbview.exe", 11519, oldDate),
            new FileEntry("draw2wmf.exe", 12543, oldDate),
            new FileEntry("drw2wmf.exe", 13567, oldDate),
            new FileEntry("ico2dib.exe", 14591, oldDate),
            new FileEntry("msp2dib.exe", 15615, oldDate),
            new FileEntry("nc.exe", 16639, oldDate),
            new FileEntry("nc.cfg", 17663, oldDate),
            new FileEntry("nc.ext", 18687, oldDate),
            new FileEntry("nc.fil", 19711, oldDate),
            new FileEntry("nc.hlp", 20735, oldDate),
            new FileEntry("nc.ico", 21759, oldDate),
            new FileEntry("nc.ini", 22783, oldDate),
            new FileEntry("nc_exit.com", 23807, oldDate),
            new FileEntry("nc_exit.doc", 24831, oldDate),
            new FileEntry("ncclean.exe", 25855, oldDate),
            new FileEntry("ncclean.ini", 26879, oldDate),
            new FileEntry("ncdd.exe", 27903, oldDate),
            new FileEntry("ncedit.exe", 28927, oldDate),
            new FileEntry("ncff.exe", 29951, oldDate),
            new FileEntry("ncff.hlp", 30975, oldDate),
            new FileEntry("nclabel.exe", 31999, oldDate),
            new FileEntry("ncmain.exe", 33023, oldDate),
            new FileEntry("ncnet.exe", 34047, oldDate),
            new FileEntry("ncsf.exe", 35071, oldDate),
            new FileEntry("ncsi.exe", 36095, oldDate),
            new FileEntry("nczip.exe", 37119, oldDate),
            new FileEntry("ncpscrip.hdr", 38143, oldDate),
            new FileEntry("norton.ini", 39167, oldDate),
            new FileEntry("packer.exe", 40191, oldDate),
            new FileEntry("paraview.exe", 41215, oldDate),
            new FileEntry("pct2dib.exe", 42239, oldDate),
            new FileEntry("playwave.exe", 43263, oldDate),
            new FileEntry("qaview.exe", 44287, oldDate),
            new FileEntry("rbview.exe", 45311, oldDate),
            new FileEntry("refview.exe", 46335, oldDate),
            new FileEntry("saver.exe", 47359, oldDate),
            new FileEntry("telemax.exe", 48383, oldDate),
            new FileEntry("telemax.dat", 49407, oldDate),
            new FileEntry("telemax.hlp", 50431, oldDate),
            new FileEntry("tif2dib.exe", 51455, oldDate),
            new FileEntry("vector.exe", 52479, oldDate),
            new FileEntry("wpb2dib.exe", 53503, oldDate),
            new FileEntry("wpv2wmf.exe", 54527, oldDate),
            new FileEntry("wpview.exe", 55551, oldDate),
        };
        return files;
    }
}

// 2. FileEntry описывает одну запись: имя, размер, дату и тип.
public class FileEntry
{
    public string Name { get; }
    public long Size { get; }
    public DateTime Modified { get; }
    public bool IsDirectory { get; }

    public FileEntry(string name, long size, DateTime modified, bool isDirectory = false)
    {
        Name = name;
        Size = size;
        Modified = modified;
        IsDirectory = isDirectory;
    }
}

// 3. Commander сортирует записи и рисует интерфейс средствами Console.
public class Commander
{
    public const int Width = 80, Height = 25, FileRows = 17, NameWidth = 12;
    private readonly string[] _lines = new string[Height];
    private bool _textOnly;

    public static string ShortName(string name, int width)
    {
        if (width <= 0) return "";
        if (name.Length <= width) return name;
        string extension = Path.GetExtension(name);
        // Оставляем начало имени, знак ~ и расширение.
        int prefix = width - extension.Length - 1;
        if (prefix > 0) return name.Substring(0, prefix) + "~" + extension;
        return name.Substring(0, width - 1) + "~";
    }

    public static List<FileEntry> SortFiles(List<FileEntry> files)
    {
        // Сортируем копию списка, чтобы не менять исходный порядок.
        List<FileEntry> sorted = new List<FileEntry>(files);
        sorted.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name));
        sorted.Insert(0, new FileEntry("..", 0,
            new DateTime(2002, 10, 11, 19, 48, 0), true));
        return sorted;
    }

    public static ConsoleColor FileColor(FileEntry file)
    {
        return file.IsDirectory ? ConsoleColor.White : ConsoleColor.Cyan;
    }

    public void Draw(List<FileEntry> files, bool textOnly = false)
    {
        _textOnly = textOnly || Console.IsOutputRedirected;
        for (int y = 0; y < Height; y++) _lines[y] = new string(' ', Width);
        if (!_textOnly)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.CursorVisible = false;
        }
        List<FileEntry> sorted = SortFiles(files);
        WriteAt(0, 0, new string(' ', Width),
            ConsoleColor.Black, ConsoleColor.DarkCyan);
        string[] menu = { "Левая", "Файл", "Диск", "Команды", "Правая" };
        int[] positions = { 4, 13, 21, 29, 40 };
        for (int i = 0; i < menu.Length; i++)
        {
            WriteAt(positions[i], 0, menu[i],
                ConsoleColor.Black, ConsoleColor.DarkCyan);
            WriteAt(positions[i], 0, menu[i].Substring(0, 1),
                ConsoleColor.Yellow, ConsoleColor.DarkCyan);
        }
        WriteAt(75, 0, "08:30", ConsoleColor.Black, ConsoleColor.Cyan);
        Frame( 0, new[] { 13, 26 });
        Frame( 40, new[] { 13, 23, 32 });
        WriteAt(1, 2, "C:↓ Имя", ConsoleColor.Yellow);
        WriteAt(17, 2, "Имя", ConsoleColor.Yellow);
        WriteAt(30, 2, "Имя", ConsoleColor.Yellow);
        WriteAt(41, 2, "C:↓ Имя", ConsoleColor.Yellow);
        WriteAt(55, 2, "Размер", ConsoleColor.Yellow);
        WriteAt(65, 2, "Дата", ConsoleColor.Yellow);
        WriteAt(73, 2, "Время", ConsoleColor.Yellow);
        // Левая панель: заполняем сверху вниз, затем следующую колонку.
        // Math.Min ограничивает список числом свободных мест.
        for (int i = 0; i < Math.Min(sorted.Count, FileRows * 3); i++)
        {
            FileEntry file = sorted[i];
            int x = 1 + i / FileRows * 13;
            int y = 3 + i % FileRows;
            WriteAt(x, y, ShortName(file.Name, NameWidth)
                .PadRight(NameWidth), FileColor(file));
        }
        // Правая панель: каждая запись занимает одну строку из 4 полей.
        for (int i = 0; i < Math.Min(sorted.Count, FileRows); i++)
        {
            FileEntry file = sorted[i];
            // Первую строку выделяем фоном, как в Norton Commander.
            ConsoleColor bg = i == 0
                ? ConsoleColor.Cyan : ConsoleColor.DarkBlue;
            ConsoleColor fg = i == 0 ? ConsoleColor.Black : FileColor(file);
            if (i == 0)
            {
                // Выделение занимает всю строку, включая разделители.
                WriteAt(41, 3, new string(' ', 38), fg, bg);
                foreach (int x in new[] { 53, 63, 72 })
                    WriteAt(x, 3, "│", fg, bg);
            }
            WriteAt(41, 3 + i, ShortName(file.Name, NameWidth)
                .PadRight(NameWidth), fg, bg);
            string size = file.IsDirectory ? "►КАТАЛОГ◄"
                : file.Size.ToString(CultureInfo.InvariantCulture);
            WriteAt(54, 3 + i, size.PadLeft(9), fg, bg);
            WriteAt(64, 3 + i, file.Modified.ToString(
                "dd.MM.yy", CultureInfo.InvariantCulture), fg, bg);
            WriteAt(73, 3 + i, file.Modified.ToString(
                "HH:mm", CultureInfo.InvariantCulture).PadLeft(6), fg, bg);
        }
        // Нижние строки: сведения о выделенной записи, команда, клавиши.
        string status = "..".PadRight(12) + " ►КАТАЛОГ◄ 11.10.02  19:48";
        WriteAt(1, 21, status.PadRight(38));
        WriteAt(41, 21, status.PadRight(38));
        WriteAt(0, 23, @"C:\NC>", ConsoleColor.Gray,
            ConsoleColor.Black);
        string[] keys = { "Помощь", "Вызов", "Чтение", "Правка",
            "Копия", "НовИмя", "НовКат", "Удал-е", "Меню", "Выход" };
        for (int i = 0; i < keys.Length; i++)
        {
            string number = (i + 1).ToString();
            WriteAt(i * 8, 24, number,
                ConsoleColor.Gray, ConsoleColor.Black);
            WriteAt(i * 8 + number.Length, 24, keys[i].PadRight(6),
                ConsoleColor.Black, ConsoleColor.DarkCyan);
        }
        WriteAt(79, 24, " ", ConsoleColor.Gray, ConsoleColor.Black);
        if (_textOnly)
            Console.WriteLine(string.Join('\n', _lines));
        else
        {
            Console.SetCursorPosition(6, 23);
            Console.ResetColor();
        }
    }

    // Рамки обеих панелей одинаковые; x задаёт начало панели.
    private void Frame(int x, int[] dividers)
    {
        // Обе панели рисуем одинаково: отличаются только разделители.
        for (int y = 1; y <= 22; y++)
            WriteAt(x, y, "║" + new string(' ', 38) + "║");
        WriteAt(x, 1, "╔" + new string('═', 38) + "╗");
        WriteAt(x, 22, "╚" + new string('═', 38) + "╝");
        WriteAt(x, 20, "╟" + new string('─', 38) + "╢");
        foreach (int divider in dividers)
        {
            WriteAt(x + divider, 1, "╤");
            for (int y = 2; y < 20; y++)
                WriteAt(x + divider, y, "│");
            WriteAt(x + divider, 20, "┴");
        }
        WriteAt(x + 15, 1, @" C:\NC ");
        if (x == 40)
            WriteAt(x + 15, 1, @" C:\NC ",
                ConsoleColor.Black, ConsoleColor.Cyan);
    }

    private void WriteAt(int x, int y, string text,
        ConsoleColor foreground = ConsoleColor.Cyan,
        ConsoleColor background = ConsoleColor.DarkBlue)
    {
        if (_textOnly)
        {
            // При проверке или выводе в файл собираем обычные строки без цветов.
            _lines[y] = _lines[y].Remove(x, text.Length).Insert(x, text);
            return;
        }
        // Последний символ пустой: не печатаем его, чтобы окно не прокрутилось.
        if (y == Height - 1 && x + text.Length == Width)
            text = text.Substring(0, text.Length - 1);
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.Write(text);
    }
}
