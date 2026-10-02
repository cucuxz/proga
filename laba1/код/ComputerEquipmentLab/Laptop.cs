using System.Globalization;

namespace ComputerEquipmentLab;

// Второй потомок того же базового класса: ноутбук с экраном и аккумулятором.
public class Laptop : ComputerEquipment
{
    public const int MinimumRamGb = 1;
    public const int MinimumBatteryCapacityMah = 1;
    public const decimal DefaultScreenSizeInches = 1m;

    // Дополнительные поля ноутбука тоже изменяются через свойства.
    private string _processor = string.Empty;
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
                throw new ArgumentOutOfRangeException(nameof(RamGb),
                    "Объем памяти должен быть положительным.");
            _ramGb = value;
        }
    }

    public decimal ScreenSizeInches
    {
        get => _screenSizeInches;
        set
        {
            if (value <= decimal.Zero)
                throw new ArgumentOutOfRangeException(nameof(ScreenSizeInches),
                    "Диагональ должна быть положительной.");
            _screenSizeInches = value;
        }
    }

    public int BatteryCapacityMah
    {
        get => _batteryCapacityMah;
        set
        {
            if (value < MinimumBatteryCapacityMah)
                throw new ArgumentOutOfRangeException(nameof(BatteryCapacityMah),
                    "Емкость аккумулятора должна быть положительной.");
            _batteryCapacityMah = value;
        }
    }

    public Laptop()
        : this(Unspecified, Unspecified, decimal.Zero, DateTime.Today.Year,
               Unspecified, MinimumRamGb, DefaultScreenSizeInches, MinimumBatteryCapacityMah)
    {
    }

    // Общие параметры передаём в base, остальные сохраняем здесь.
    public Laptop(string brand, string model, decimal price, int year,
                  string processor, int ramGb, decimal screenSizeInches, int batteryCapacityMah)
        : base(brand, model, price, year)
    {
        Processor = processor;
        RamGb = ramGb;
        ScreenSizeInches = screenSizeInches;
        BatteryCapacityMah = batteryCapacityMah;
    }

    // При вызове через ComputerEquipment будет выбран именно этот override.
    public override string GetInfo()
    {
        string screen = ScreenSizeInches.ToString("0.##", CultureInfo.InvariantCulture);
        return $"Ноутбук | {GetCommonInfo()} | Процессор: {Processor} | "
            + $"ОЗУ: {RamGb} ГБ | Экран: {screen} дюйм. | "
            + $"Аккумулятор: {BatteryCapacityMah} мА·ч";
    }
}
