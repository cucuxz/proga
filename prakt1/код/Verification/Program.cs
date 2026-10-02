using NortonCommanderConsole;

int count = 0;
void Check(bool result, string name)
{
    if (!result) throw new Exception(name);
    Console.WriteLine($"PASS {++count}: {name}");
}
string[] Draw(List<FileEntry> files)
{
    TextWriter previous = Console.Out;
    using StringWriter writer = new StringWriter();
    try
    {
        Console.SetOut(writer);
        new Commander().Draw(files, textOnly: true);
        return writer.ToString().TrimEnd('\r', '\n').Split('\n');
    }
    finally { Console.SetOut(previous); }
}
string Field(string[] lines, int x, int y, int width) => lines[y].Substring(x, width);

Check(Commander.ShortName("MyFileWithLongName.txt", 12) ==
    "MyFileW~.txt", "Long name preserves extension and ~");
Check(Commander.ShortName("nc.exe", 12) == "nc.exe",
    "Short name remains unchanged");
Check(Commander.ShortName("abcdefgh", 1) == "~" &&
    Commander.ShortName("a", 0) == "" &&
    Commander.ShortName("file.verylongextension", 12).Length == 12,
    "Narrow field and long extension do not overflow");
var input = new List<FileEntry> {
    new("z.txt", 5, new DateTime(2026, 1, 2, 3, 4, 0)),
    new("A.txt", 12, new DateTime(2026, 1, 2, 3, 4, 0)),
    new("middle", 0, DateTime.MinValue, true) };
var small = Draw(input);
Check(Field(small, 1, 4, 12).Trim() == "A.txt" &&
    Field(small, 1, 5, 12).Trim() == "middle" &&
    Field(small, 1, 6, 12).Trim() == "z.txt" &&
    input[0].Name == "z.txt", "Alphabetical order, original list unchanged");
Check(Field(small, 54, 4, 9).Trim() == "12" &&
    Field(small, 64, 4, 8) == "02.01.26" &&
    Field(small, 73, 4, 6).Trim() == "03:04",
    "Right panel shows size, date and time in separate fields");
Check(Commander.FileColor(input[2]) == ConsoleColor.White &&
    Commander.FileColor(input[1]) == ConsoleColor.Cyan,
    "Directories and files have different colours");
var many = Enumerable.Range(0, 100).Reverse().Select(i =>
    new FileEntry($"f{i:D3}.txt", i, DateTime.MinValue)).ToList();
var full = Draw(many);
Check(Field(full, 1, 19, 12).Trim() == "f015.txt" &&
    Field(full, 14, 3, 12).Trim() == "f016.txt" &&
    Field(full, 27, 19, 12).Trim() == "f049.txt" &&
    Field(full, 41, 19, 12).Trim() == "f015.txt" &&
    full[20][0] == '╟',
    "51 left slots, 17 right slots, overflow preserves frame");
Check(Draw(new())[3][1] == '.' &&
    Draw(new())[4][1] == ' ',
    "Empty list displays only parent directory");
var demo = Draw(NortonCommanderConsole.Program.CreateFiles());
Check(NortonCommanderConsole.Program.CreateFiles().Count == 66 && demo.Length == 25 &&
    demo.All(r => r.Length == 80) &&
    demo[1][0] == '╔' &&
    demo[22][79] == '╝' &&
    demo[24][79] == ' ',
    "Demo data count, exact 80x25 canvas and safe bottom-right cell");
Console.WriteLine($"TOTAL: {count}/{count} passed");
