using ComputerEquipmentLab;

namespace ComputerEquipmentLab.Tests;

internal static class Program
{
    private static int Main()
    {
        // Независимые модульные тесты без внешних пакетов и сетевого восстановления.
        (string Name, Action Run)[] tests =
        {
            ("Базовый конструктор по умолчанию", BaseDefaults),
            ("Базовый конструктор с параметрами", BaseParameterized),
            ("Изменение общих свойств", BasePropertiesCanChange),
            ("Пустые строки запрещены", EmptyTextRejected),
            ("Отрицательная цена запрещена", NegativePriceRejected),
            ("Границы года выпуска", YearBoundaries),
            ("Конструктор ПК по умолчанию", PersonalComputerDefaults),
            ("Свойства ПК и переопределенный вывод", PersonalComputerProperties),
            ("Недопустимый процессор ПК", InvalidPersonalComputerProcessor),
            ("Недопустимая память ПК", InvalidPersonalComputerRam),
            ("Конструктор ноутбука по умолчанию", LaptopDefaults),
            ("Свойства ноутбука и переопределенный вывод", LaptopProperties),
            ("Недопустимый процессор ноутбука", InvalidLaptopProcessor),
            ("Недопустимая память ноутбука", InvalidLaptopRam),
            ("Недопустимая диагональ", InvalidScreenSize),
            ("Недопустимая емкость аккумулятора", InvalidBatteryCapacity),
            ("Полиморфизм и ToString", PolymorphismAndToString),
            ("Порядок списка после удаления", OrderedListAfterRemoval)
        };

        int failures = 0;
        foreach ((string name, Action run) in tests)
        {
            try
            {
                run();
                Console.WriteLine($"PASS {name}");
            }
            catch (Exception exception)
            {
                failures++;
                Console.WriteLine($"FAIL {name}: {exception.GetType().Name}: {exception.Message}");
            }
        }

        Console.WriteLine($"Итого: {tests.Length - failures}/{tests.Length} тестов пройдено.");
        return failures == 0 ? 0 : 1;
    }

    private static void BaseDefaults()
    {
        ComputerEquipment item = new();
        Check.Equal(ComputerEquipment.Unspecified, item.Brand);
        Check.Equal(ComputerEquipment.Unspecified, item.Model);
        Check.Equal(decimal.Zero, item.Price);
        Check.Equal(DateTime.Today.Year, item.Year);
    }

    private static void BaseParameterized()
    {
        ComputerEquipment item = new("  Acme  ", "Tower", 45000.50m, 2024);
        Check.Equal("Acme", item.Brand);
        Check.Equal("Tower", item.Model);
        Check.Equal(45000.50m, item.Price);
        Check.Equal(2024, item.Year);
        Check.Contains("Компьютерная техника", item.GetInfo());
        Check.Contains("45000.50 руб.", item.GetInfo());
    }

    private static void BasePropertiesCanChange()
    {
        ComputerEquipment item = new();
        item.Brand = "  Lenovo ";
        item.Model = "ThinkCentre";
        item.Price = 0m;
        item.Year = ComputerEquipment.MinimumYear;
        Check.Equal("Lenovo", item.Brand);
        Check.Equal("ThinkCentre", item.Model);
        Check.Equal(0m, item.Price);
        Check.Equal(ComputerEquipment.MinimumYear, item.Year);
    }

    private static void EmptyTextRejected()
    {
        ComputerEquipment item = new();
        Check.Throws<ArgumentException>(() => item.Brand = "   ");
        Check.Throws<ArgumentException>(() => item.Model = null!);
        Check.Equal(ComputerEquipment.Unspecified, item.Brand);
    }

    private static void NegativePriceRejected()
    {
        ComputerEquipment item = new();
        Check.Throws<ArgumentOutOfRangeException>(() => item.Price = -0.01m);
        Check.Equal(decimal.Zero, item.Price);
    }

    private static void YearBoundaries()
    {
        ComputerEquipment item = new();
        item.Year = ComputerEquipment.MinimumYear;
        item.Year = DateTime.Today.Year + ComputerEquipment.AllowedFutureYears;
        Check.Throws<ArgumentOutOfRangeException>(() => item.Year = ComputerEquipment.MinimumYear - 1);
        Check.Throws<ArgumentOutOfRangeException>(() => item.Year = DateTime.Today.Year + ComputerEquipment.AllowedFutureYears + 1);
    }

    private static void PersonalComputerDefaults()
    {
        PersonalComputer item = new();
        Check.Equal(PersonalComputer.MinimumRamGb, item.RamGb);
        Check.Equal(ComputerEquipment.Unspecified, item.Processor);
        Check.Equal(false, item.HasDiscreteGpu);
    }

    private static void PersonalComputerProperties()
    {
        PersonalComputer item = new("Dell", "OptiPlex", 60000m, 2025, "  Core i5  ", 16, true);
        item.RamGb = 32;
        item.HasDiscreteGpu = false;
        Check.Equal("Core i5", item.Processor);
        Check.Equal(32, item.RamGb);
        Check.Contains("Персональный компьютер", item.GetInfo());
        Check.Contains("Дискретная видеокарта: нет", item.GetInfo());
    }

    private static void InvalidPersonalComputerProcessor() =>
        Check.Throws<ArgumentException>(() => new PersonalComputer("Dell", "X", 1m, 2024, " ", 8, false));

    private static void InvalidPersonalComputerRam() =>
        Check.Throws<ArgumentOutOfRangeException>(() => new PersonalComputer("Dell", "X", 1m, 2024, "CPU", 0, false));

    private static void LaptopDefaults()
    {
        Laptop item = new();
        Check.Equal(Laptop.MinimumRamGb, item.RamGb);
        Check.Equal(Laptop.DefaultScreenSizeInches, item.ScreenSizeInches);
        Check.Equal(Laptop.MinimumBatteryCapacityMah, item.BatteryCapacityMah);
    }

    private static void LaptopProperties()
    {
        Laptop item = new("Asus", "Zenbook", 90000m, 2025, "Ryzen 7", 16, 14.5m, 6000);
        item.ScreenSizeInches = 15.6m;
        item.BatteryCapacityMah = 7000;
        Check.Equal("Ryzen 7", item.Processor);
        Check.Equal(16, item.RamGb);
        Check.Equal(15.6m, item.ScreenSizeInches);
        Check.Equal(7000, item.BatteryCapacityMah);
        Check.Contains("Ноутбук", item.GetInfo());
        Check.Contains("Экран: 15.6 дюйм.", item.GetInfo());
    }

    private static void InvalidLaptopProcessor() =>
        Check.Throws<ArgumentException>(() => new Laptop("Asus", "X", 1m, 2024, "", 8, 14m, 5000));

    private static void InvalidLaptopRam() =>
        Check.Throws<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 0, 14m, 5000));

    private static void InvalidScreenSize() =>
        Check.Throws<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 8, 0m, 5000));

    private static void InvalidBatteryCapacity() =>
        Check.Throws<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 8, 14m, 0));

    private static void PolymorphismAndToString()
    {
        List<ComputerEquipment> items =
        [
            new ComputerEquipment("A", "B", 0m, 2024),
            new PersonalComputer("C", "D", 1m, 2024, "CPU", 8, true),
            new Laptop("E", "F", 2m, 2024, "CPU", 16, 15m, 5000)
        ];
        Check.Contains("Компьютерная техника", items[0].GetInfo());
        Check.Contains("Персональный компьютер", items[1].GetInfo());
        Check.Contains("Ноутбук", items[2].GetInfo());
        foreach (ComputerEquipment item in items)
            Check.Equal(item.GetInfo(), item.ToString());
    }

    private static void OrderedListAfterRemoval()
    {
        List<ComputerEquipment> items = [new ComputerEquipment(), new PersonalComputer(), new Laptop()];
        items.RemoveAt(1);
        Check.Equal(2, items.Count);
        Check.Equal(typeof(ComputerEquipment), items[0].GetType());
        Check.Equal(typeof(Laptop), items[1].GetType());
    }
}

internal static class Check
{
    public static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"Ожидалось: {expected}; получено: {actual}.");
    }

    public static void Contains(string expectedPart, string actual)
    {
        if (!actual.Contains(expectedPart, StringComparison.Ordinal))
            throw new InvalidOperationException($"В строке нет фрагмента «{expectedPart}»: {actual}");
    }

    public static void Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Ожидалось {typeof(TException).Name}, получено {exception.GetType().Name}.");
        }
        throw new InvalidOperationException($"Ожидалось исключение {typeof(TException).Name}.");
    }
}
