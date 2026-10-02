using System.Globalization;
using System.Text;

namespace ComputerEquipmentLab;

internal static class Program
{
    private const int Exit = 0, Add = 1, Show = 2, Delete = 3;
    private const int GeneralType = 1, PcType = 2, LaptopType = 3;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        // Один список принимает объекты всех трёх классов.
        List<ComputerEquipment> devices = new List<ComputerEquipment>();
        Console.WriteLine("Учёт компьютерной техники");
        try
        {
            while (true)
            {
                Console.WriteLine("\n1 — добавить; 2 — показать; 3 — удалить; 0 — выход");
                int action = ReadInt("Действие: ", Exit, Delete);
                if (action == Exit) break;
                switch (action)
                {
                    case Add:
                        devices.Add(CreateDevice());
                        Console.WriteLine("Устройство добавлено.");
                        break;
                    case Show:
                        ShowDevices(devices);
                        break;
                    case Delete:
                        DeleteDevice(devices);
                        break;
                }
            }
        }
        // Если ввод закончился, завершаем работу даже посреди добавления.
        catch (EndOfStreamException) { }
        Console.WriteLine("Работа завершена.");
    }

    private static ComputerEquipment CreateDevice()
    {
        Console.WriteLine("1 — общая техника; 2 — ПК; 3 — ноутбук");
        int type = ReadInt("Тип устройства: ", GeneralType, LaptopType);
        // Эти четыре свойства одинаковы у любого устройства.
        string brand = ReadText("Производитель: ");
        string model = ReadText("Модель: ");
        decimal price = ReadDecimal("Цена в рублях: ", allowZero: true);
        int year = ReadInt("Год выпуска: ", ComputerEquipment.MinimumYear,
            DateTime.Today.Year + ComputerEquipment.AllowedFutureYears);
        if (type == GeneralType)
            return new ComputerEquipment(brand, model, price, year);

        string processor = ReadText("Процессор: ");
        int ram = ReadInt("ОЗУ в ГБ: ", PersonalComputer.MinimumRamGb, int.MaxValue);
        if (type == PcType)
        {
            bool gpu = ReadInt("Дискретная видеокарта (1 — да, 0 — нет): ", 0, 1) == 1;
            // Конструктор потомка передаёт общие значения в базовый класс.
            return new PersonalComputer(brand, model, price, year, processor, ram, gpu);
        }
        decimal screen = ReadDecimal("Экран в дюймах: ", allowZero: false);
        int battery = ReadInt("Аккумулятор в мА·ч: ",
            Laptop.MinimumBatteryCapacityMah, int.MaxValue);
        return new Laptop(brand, model, price, year, processor, ram, screen, battery);
    }

    private static void ShowDevices(List<ComputerEquipment> devices)
    {
        if (devices.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }
        for (int i = 0; i < devices.Count; i++)
        {
            // Полиморфизм: GetInfo выбирается по типу самого объекта.
            string info = devices[i].GetInfo();
            Console.WriteLine($"{i + 1}. {info.Replace(" | ", "\n   ")}");
        }
    }

    private static void DeleteDevice(List<ComputerEquipment> devices)
    {
        ShowDevices(devices);
        if (devices.Count == 0) return;
        int number = ReadInt("Номер для удаления: ", 1, devices.Count);
        // На экране номера начинаются с 1, а индексы списка — с 0.
        devices.RemoveAt(number - 1);
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
            string input = ReadLine(prompt);
            if (input.Length > 0) return input;
            Console.WriteLine("Введите непустое значение.");
        }
    }

    private static int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            // TryParse проверяет число и не выбрасывает ошибку при вводе текста.
            string input = ReadLine(prompt);
            if (int.TryParse(input, out int value) && value >= minimum && value <= maximum)
                return value;
            Console.WriteLine($"Введите целое число от {minimum} до {maximum}.");
        }
    }

    private static decimal ReadDecimal(string prompt, bool allowZero)
    {
        while (true)
        {
            // Принимаем и запятую, и точку: 14,5 и 14.5 означают одно число.
            string input = ReadLine(prompt).Replace(',', '.');
            if (decimal.TryParse(input,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out decimal value)
                && (allowZero ? value >= 0 : value > 0))
                return value;
            Console.WriteLine(allowZero ? "Введите число не меньше 0."
                : "Введите число больше 0.");
        }
    }
}
