using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace CollectionsPractice;

// Класс ComputerEquipment взят из лабораторной №1 и дополнен интерфейсами.
public class ComputerEquipment : IComparable, ICloneable
{
    public const int MinimumYear = 1900;
    public const int AllowedFutureYears = 1;
    public const string Unspecified = "Не указано";
    private string _brand = "", _model = "";
    private decimal _price;
    private int _year;

    // Свойства сохраняют те же проверки, что и в первой лабораторной.
    public string Brand
    {
        get => _brand;
        set => _brand = ValidateText(value);
    }
    public string Model
    {
        get => _model;
        set => _model = ValidateText(value);
    }
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(Price));
            _price = value;
        }
    }
    public int Year
    {
        get => _year;
        set
        {
            if (value < MinimumYear || value > DateTime.Today.Year + AllowedFutureYears)
                throw new ArgumentOutOfRangeException(nameof(Year));
            _year = value;
        }
    }

    // Изменяемый вложенный список позволяет наглядно проверить глубокую копию.
    public List<string> Accessories { get; } = new List<string>();

    public ComputerEquipment()
        : this(Unspecified, Unspecified, 0, DateTime.Today.Year) { }

    public ComputerEquipment(string brand, string model, decimal price, int year)
    {
        Brand = brand;
        Model = model;
        Price = price;
        Year = year;
    }

    private static string ValidateText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Название не может быть пустым.");
        return value.Trim();
    }

    // IComparable задаёт обычный порядок: от меньшей цены к большей.
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is not ComputerEquipment other)
            throw new ArgumentException("Можно сравнивать только компьютерную технику.");
        return Price.CompareTo(other.Price);
    }

    // Копируем и сам объект, и вложенный список. Строки в C# неизменяемы.
    public object Clone()
    {
        ComputerEquipment copy = new ComputerEquipment(Brand, Model, Price, Year);
        copy.Accessories.AddRange(Accessories);
        return copy;
    }

    public virtual string GetInfo()
        => $"{Brand} {Model}; {Price:0.00} руб.; {Year}; "
           + "комплект: " + string.Join(", ", Accessories);
    public override string ToString() => GetInfo();
}

// IComparer задаёт другой порядок: сначала производитель, затем модель.
public sealed class EquipmentComparer : IComparer, IComparer<ComputerEquipment>
{
    public int Compare(ComputerEquipment? x, ComputerEquipment? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        int result = StringComparer.OrdinalIgnoreCase.Compare(x.Brand, y.Brand);
        return result != 0 ? result : StringComparer.OrdinalIgnoreCase.Compare(x.Model, y.Model);
    }

    // Необобщённая версия нужна для IComparer; List<T>.Sort использует обобщённую.
    int IComparer.Compare(object? x, object? y)
    {
        if ((x is not null && x is not ComputerEquipment)
            || (y is not null && y is not ComputerEquipment))
            throw new ArgumentException("Сравниваются только объекты техники.");
        return Compare((ComputerEquipment?)x, (ComputerEquipment?)y);
    }
}

// Program содержит меню и четыре демонстрации по пунктам задания.
public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (args.Contains("--demo"))
        {
            RunAll(true);
            return;
        }
        while (true)
        {
            Console.WriteLine("\nПрактическая работа 3 — вариант 1");
            Console.WriteLine("1 — ArrayList; 2 — Stack<char> и List<char>");
            Console.WriteLine("3 — Техника и интерфейсы; 4 — ObservableCollection");
            Console.Write("5 — Все примеры; 0 — Выход. Выбор: ");
            string? choice = Console.ReadLine();
            if (choice is null || choice == "0") return;
            try
            {
                switch (choice)
                {
                    case "1": ShowArrayList(false); break;
                    case "2": ShowCharacters(false); break;
                    case "3": ShowEquipment(false); break;
                    case "4": ShowObservable(); break;
                    case "5": RunAll(false); break;
                    default: Console.WriteLine("Введите пункт от 0 до 5."); break;
                }
            }
            catch (EndOfStreamException) { return; }
        }
    }

    private static void RunAll(bool demo)
    {
        ShowArrayList(demo);
        ShowCharacters(demo);
        ShowEquipment(demo);
        ShowObservable();
    }

    // ArrayList хранит object, поэтому числа и строка находятся в одной коллекции.
    public static ArrayList CreateArrayList(Random random)
    {
        ArrayList items = new ArrayList();
        for (int i = 0; i < 5; i++) items.Add(random.Next(1, 100));
        items.Add("C#");
        return items;
    }

    public static void ShowArrayList(bool demo)
    {
        Console.WriteLine("\n1. Необобщённая коллекция ArrayList");
        // Фиксированный seed нужен только для воспроизводимого примера в отчёте.
        ArrayList items = CreateArrayList(demo ? new Random(1) : new Random());
        Print("Пять случайных чисел и строка", items);
        object removed = demo ? items[0]! : ReadValue("Что удалить (число или строка): ");
        object searched = demo ? "C#" : ReadValue("Что найти (число или строка): ");
        bool existed = items.Contains(removed);
        items.Remove(removed); // Remove удаляет первое совпадение по значению.
        Console.WriteLine($"Удаление {removed}: {(existed ? "удалён" : "не найден")}");
        Print($"После удаления; количество = {items.Count}", items);
        Console.WriteLine($"Поиск {searched}: {items.Contains(searched)}");
    }

    // Стек работает по LIFO: последним добавили — первым извлекли.
    public static void RemoveTop<T>(Stack<T> stack, int count)
    {
        if (count < 0 || count > stack.Count)
            throw new ArgumentOutOfRangeException(nameof(count));
        for (int i = 0; i < count; i++) stack.Pop();
    }

    public static void ShowCharacters(bool demo)
    {
        Console.WriteLine("\n2. Stack<char> → List<char>");
        Stack<char> stack = new Stack<char>();
        foreach (char item in new[] { 'A', 'B', 'C', 'D', 'E' }) stack.Push(item);
        Print("Стек от вершины к основанию", stack);
        int count = demo ? 2 : ReadNumber("Сколько элементов удалить (0–5): ", 0, stack.Count);
        RemoveTop(stack, count);
        Print($"После удаления {count} элементов", stack);
        // У Stack<T> единственный метод добавления — Push; Add у него отсутствует.
        stack.Push('X');
        stack.Push('Y');
        Print("После Push('X') и Push('Y')", stack);
        List<char> list = new List<char>(stack); // Сохраняется порядок обхода стека.
        Print("Вторая коллекция List<char>", list);
        char target = demo ? 'X' : ReadCharacter("Символ для поиска: ");
        Console.WriteLine($"Поиск {target} в списке: {list.Contains(target)}");
    }

    public static Stack<ComputerEquipment> CreateEquipmentStack()
    {
        Stack<ComputerEquipment> stack = new Stack<ComputerEquipment>();
        stack.Push(new ComputerEquipment("Lenovo", "ThinkPad", 70000, 2024));
        stack.Push(new ComputerEquipment("Dell", "OptiPlex", 50000, 2023));
        stack.Push(new ComputerEquipment("Acer", "Aspire", 60000, 2024));
        foreach (ComputerEquipment item in stack) item.Accessories.Add("Кабель");
        return stack;
    }

    public static ComputerEquipment? FindByModel(IEnumerable<ComputerEquipment> items, string model)
    {
        foreach (ComputerEquipment item in items)
            if (string.Equals(item.Model, model, StringComparison.OrdinalIgnoreCase)) return item;
        return null;
    }

    public static void ShowEquipment(bool demo)
    {
        Console.WriteLine("\n3. Stack<ComputerEquipment> → List<ComputerEquipment>");
        Stack<ComputerEquipment> stack = CreateEquipmentStack();
        Print("Исходный стек техники", stack);
        int count = demo ? 1 : ReadNumber("Сколько объектов удалить (0–3): ", 0, stack.Count);
        RemoveTop(stack, count);
        Print($"После удаления {count} объектов", stack);
        stack.Push(new ComputerEquipment("Asus", "Zenbook", 80000, 2025));
        stack.Push(new ComputerEquipment("HP", "ProBook", 55000, 2024));
        Print("После двух вызовов Push", stack);
        List<ComputerEquipment> list = new List<ComputerEquipment>(stack);
        Print("Вторая коллекция List<ComputerEquipment>", list);
        string model = demo ? "OptiPlex" : ReadText("Модель для поиска: ");
        ComputerEquipment? found = FindByModel(list, model);
        Console.WriteLine($"Поиск {model}: {(found is null ? "не найдена" : found.ToString())}");

        // Sort() использует IComparable; Sort(comparer) — отдельный IComparer.
        list.Sort();
        Print("IComparable: сортировка по возрастанию цены", list);
        list.Sort(new EquipmentComparer());
        Print("IComparer: сортировка по производителю и модели", list);

        ComputerEquipment original = list[0];
        original.Accessories.Add("Мышь");
        ComputerEquipment copy = (ComputerEquipment)original.Clone();
        copy.Model = "Копия";
        copy.Accessories[0] = "Клавиатура";
        Console.WriteLine("ICloneable: изменяем модель и комплект только у копии");
        Console.WriteLine("Оригинал: " + original);
        Console.WriteLine("Копия:    " + copy);
        Console.WriteLine("Общий объект: " + ReferenceEquals(original, copy));
        Console.WriteLine("Общий список комплекта: " + ReferenceEquals(original.Accessories, copy.Accessories));
    }

    public static void ShowObservable()
    {
        Console.WriteLine("\n4. ObservableCollection<ComputerEquipment>");
        ObservableCollection<ComputerEquipment> items = new ObservableCollection<ComputerEquipment>();
        // В .NET событие называется CollectionChanged (в задании опечатка).
        items.CollectionChanged += OnCollectionChanged;
        ComputerEquipment first = new ComputerEquipment("Lenovo", "ThinkPad", 70000, 2024);
        items.Add(first);
        items.Add(new ComputerEquipment("Acer", "Aspire", 60000, 2024));
        items.Remove(first);
        Print("Оставшиеся объекты", items);
        items.CollectionChanged -= OnCollectionChanged;
    }

    public static void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine($"Событие CollectionChanged: {e.Action}");
        if (e.NewItems is not null) Print("Добавлены", e.NewItems);
        if (e.OldItems is not null) Print("Удалены", e.OldItems);
    }

    // Все коллекции выводятся одним foreach; их содержимое при обходе не меняется.
    public static void Print(string title, IEnumerable items)
    {
        Console.WriteLine(title + ":");
        foreach (object? item in items) Console.WriteLine("  " + item);
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    private static object ReadValue(string prompt)
    {
        string text = ReadText(prompt);
        return int.TryParse(text, out int number) ? number : text;
    }

    private static int ReadNumber(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            string text = ReadText(prompt);
            if (int.TryParse(text, out int value) && value >= minimum && value <= maximum) return value;
            Console.WriteLine($"Введите целое число от {minimum} до {maximum}.");
        }
    }

    private static char ReadCharacter(string prompt)
    {
        while (true)
        {
            string text = ReadText(prompt);
            if (text.Length == 1) return text[0];
            Console.WriteLine("Введите ровно один символ.");
        }
    }
}
