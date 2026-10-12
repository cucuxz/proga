using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ComputerEquipmentLab;

// Весь код в одном файле: сначала три класса техники, затем Program с меню.
// 1. Базовый класс хранит общие характеристики всех устройств.
public class ComputerEquipment
{
    public const int MinimumYear = 1900;
    public const int AllowedFutureYears = 1;
    public const string Unspecified = "Не указано";

    // private скрывает поля; обращаться к ним снаружи можно через свойства.
    private string _brand = "";
    private string _model = "";
    private decimal _price;
    private int _year;

    public string Brand
    {
        get => _brand; // get читает поле, set проверяет и сохраняет новое значение.
        set => _brand = ValidateRequiredText(value, nameof(Brand));
    }
    public string Model
    {
        get => _model;
        set => _model = ValidateRequiredText(value, nameof(Model));
    }
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Price), "Цена не может быть отрицательной.");
            _price = value;
        }
    }
    public int Year
    {
        get => _year;
        set
        {
            if (value < MinimumYear || value > DateTime.Today.Year + AllowedFutureYears)
                throw new ArgumentOutOfRangeException(nameof(Year), "Год выпуска вне допустимого диапазона.");
            _year = value;
        }
    }

    // this вызывает второй конструктор этого же класса с готовыми значениями.
    public ComputerEquipment()
        : this(Unspecified, Unspecified, 0, DateTime.Today.Year) { }

    public ComputerEquipment(string brand, string model, decimal price, int year)
    {
        // Через свойства проверки выполняются и при создании объекта.
        Brand = brand;
        Model = model;
        Price = price;
        Year = year;
    }

    // protected разрешает потомкам пользоваться этой проверкой.
    protected static string ValidateRequiredText(string? text, string property)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Значение не может быть пустым.", property);
        return text.Trim();
    }

    protected string GetCommonInfo()
    {
        // decimal подходит для цены; формат 0.00 выводит две цифры после точки.
        string price = Price.ToString("0.00", CultureInfo.InvariantCulture);
        return $"Производитель: {Brand} | Модель: {Model} | Цена: {price} руб. | Год: {Year}";
    }

    // virtual позволяет потомкам изменить вывод сведений о себе.
    public virtual string GetInfo() => "Компьютерная техника | " + GetCommonInfo();
    public override string ToString() => GetInfo();
}

// 2. ПК наследует общие свойства и добавляет процессор, память и видеокарту.
public class PersonalComputer : ComputerEquipment
{
    public const int MinimumRamGb = 1;
    private string _processor = "";
    private int _ramGb;

    public string Processor
    {
        get => _processor;
        set => _processor = ValidateRequiredText(value, nameof(Processor));
    }
    public int RamGb
    {
        get => _ramGb;
        set
        {
            if (value < MinimumRamGb)
                throw new ArgumentOutOfRangeException(nameof(RamGb), "Объем памяти должен быть положительным.");
            _ramGb = value;
        }
    }
    // Автосвойство: оба значения bool допустимы, проверка не требуется.
    public bool HasDiscreteGpu { get; set; }

    public PersonalComputer()
        : this(Unspecified, Unspecified, 0, DateTime.Today.Year, Unspecified, MinimumRamGb, false) { }

    public PersonalComputer(string brand, string model, decimal price, int year,
                            string processor, int ramGb, bool hasDiscreteGpu)
        : base(brand, model, price, year) // base заполняет общие свойства в родителе.
    {
        Processor = processor;
        RamGb = ramGb;
        HasDiscreteGpu = hasDiscreteGpu;
    }

    // override заменяет базовый GetInfo выводом характеристик ПК.
    public override string GetInfo()
    {
        string gpu = HasDiscreteGpu ? "да" : "нет";
        return $"Персональный компьютер | {GetCommonInfo()} | Процессор: {Processor} | "
            + $"ОЗУ: {RamGb} ГБ | Дискретная видеокарта: {gpu}";
    }
}

// 3. Ноутбук — второй потомок с собственными характеристиками.
public class Laptop : ComputerEquipment
{
    public const int MinimumRamGb = 1;
    public const int MinimumBatteryCapacityMah = 1;
    public const decimal DefaultScreenSizeInches = 1m;
    private string _processor = "";
    private int _ramGb;
    private decimal _screenSizeInches;
    private int _batteryCapacityMah;

    public string Processor
    {
        get => _processor;
        set => _processor = ValidateRequiredText(value, nameof(Processor));
    }
    public int RamGb
    {
        get => _ramGb;
        set
        {
            if (value < MinimumRamGb)
                throw new ArgumentOutOfRangeException(nameof(RamGb), "Объем памяти должен быть положительным.");
            _ramGb = value;
        }
    }
    public decimal ScreenSizeInches
    {
        get => _screenSizeInches;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(ScreenSizeInches), "Диагональ должна быть положительной.");
            _screenSizeInches = value;
        }
    }
    public int BatteryCapacityMah
    {
        get => _batteryCapacityMah;
        set
        {
            if (value < MinimumBatteryCapacityMah)
                throw new ArgumentOutOfRangeException(nameof(BatteryCapacityMah), "Емкость аккумулятора должна быть положительной.");
            _batteryCapacityMah = value;
        }
    }

    public Laptop()
        : this(Unspecified, Unspecified, 0, DateTime.Today.Year,
               Unspecified, MinimumRamGb, DefaultScreenSizeInches, MinimumBatteryCapacityMah) { }

    public Laptop(string brand, string model, decimal price, int year,
                  string processor, int ramGb, decimal screenSizeInches, int batteryCapacityMah)
        : base(brand, model, price, year)
    {
        Processor = processor;
        RamGb = ramGb;
        ScreenSizeInches = screenSizeInches;
        BatteryCapacityMah = batteryCapacityMah;
    }

    public override string GetInfo()
    {
        string screen = ScreenSizeInches.ToString("0.##", CultureInfo.InvariantCulture);
        return $"Ноутбук | {GetCommonInfo()} | Процессор: {Processor} | "
            + $"ОЗУ: {RamGb} ГБ | Экран: {screen} дюйм. | Аккумулятор: {BatteryCapacityMah} мА·ч";
    }
}

// 4. Меню работает с одним списком, в котором хранятся все три типа устройств.
internal static class Program
{
    private static readonly List<ComputerEquipment> devices = new List<ComputerEquipment>();

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Учёт компьютерной техники");
        try
        {
            while (true)
            {
                Console.WriteLine("\n1 — добавить; 2 — показать; 3 — удалить; 0 — выход");
                int action = ReadInt("Действие: ", 0, 3);
                if (action == 0) break;
                // Номер пункта выбирает обычное действие через switch.
                switch (action)
                {
                    case 1:
                        devices.Add(CreateDevice());
                        Console.WriteLine("Устройство добавлено.");
                        break;
                    case 2: ShowDevices(); break;
                    case 3: DeleteDevice(); break;
                }
            }
        }
        // Конец ввода завершает программу, даже если ввод объекта не закончен.
        catch (EndOfStreamException) { }
        Console.WriteLine("Работа завершена.");
    }

    private static ComputerEquipment CreateDevice()
    {
        Console.WriteLine("1 — общая техника; 2 — ПК; 3 — ноутбук");
        int type = ReadInt("Тип устройства: ", 1, 3);
        // Сначала читаем четыре общих свойства, затем свойства выбранного типа.
        string brand = ReadText("Производитель: ");
        string model = ReadText("Модель: ");
        decimal price = ReadDecimal("Цена в рублях: ", true);
        int year = ReadInt("Год выпуска: ", ComputerEquipment.MinimumYear,
            DateTime.Today.Year + ComputerEquipment.AllowedFutureYears);
        if (type == 1) return new ComputerEquipment(brand, model, price, year);

        string processor = ReadText("Процессор: ");
        int ram = ReadInt("ОЗУ в ГБ: ", 1, int.MaxValue);
        if (type == 2)
        {
            bool gpu = ReadInt("Дискретная видеокарта (1 — да, 0 — нет): ", 0, 1) == 1;
            return new PersonalComputer(brand, model, price, year, processor, ram, gpu);
        }
        decimal screen = ReadDecimal("Экран в дюймах: ", false);
        int battery = ReadInt("Аккумулятор в мА·ч: ", 1, int.MaxValue);
        return new Laptop(brand, model, price, year, processor, ram, screen, battery);
    }

    private static void ShowDevices()
    {
        if (devices.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }
        for (int i = 0; i < devices.Count; i++)
        {
            // Полиморфизм: вызывается GetInfo того класса, к которому относится объект.
            string info = devices[i].GetInfo();
            Console.WriteLine($"{i + 1}. {info.Replace(" | ", "\n   ")}");
        }
    }

    private static void DeleteDevice()
    {
        ShowDevices();
        if (devices.Count == 0) return;
        int number = ReadInt("Номер для удаления: ", 1, devices.Count);
        devices.RemoveAt(number - 1); // Номера на экране с 1, индексы списка с 0.
        Console.WriteLine("Устройство удалено.");
    }

    private static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (input == null) throw new EndOfStreamException();
        return input.Trim();
    }

    private static string ReadText(string prompt)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (text.Length > 0) return text;
            Console.WriteLine("Введите непустое значение.");
        }
    }

    private static int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            // TryParse возвращает false при вводе текста; запрос повторяется.
            if (int.TryParse(ReadLine(prompt), out int value) && value >= minimum && value <= maximum)
                return value;
            Console.WriteLine($"Введите целое число от {minimum} до {maximum}.");
        }
    }

    private static decimal ReadDecimal(string prompt, bool allowZero)
    {
        while (true)
        {
            // Разрешены и точка, и запятая. Для цены ноль допустим, для экрана — нет.
            string text = ReadLine(prompt).Replace(',', '.');
            if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out decimal value)
                && (allowZero ? value >= 0 : value > 0))
                return value;
            Console.WriteLine(allowZero ? "Введите число не меньше 0." : "Введите число больше 0.");
        }
    }
}
