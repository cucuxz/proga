using System.Globalization;

namespace ComputerEquipmentLab;

// Базовый класс: здесь находятся общие характеристики всех устройств.
public class ComputerEquipment
{
    public const int MinimumYear = 1900;
    public const int AllowedFutureYears = 1;
    public const string Unspecified = "Не указано";

    // Поля закрыты. Снаружи доступ к ним идёт через свойства с проверкой.
    private string _brand = string.Empty;
    private string _model = string.Empty;
    private decimal _price;
    private int _year;

    public string Brand
    {
        get => _brand;
        set => _brand = ValidateRequiredText(value, nameof(Brand));
    }

    public string Model
    {
        get => _model;
        set => _model = ValidateRequiredText(value, nameof(Model));
    }

    // decimal хранит денежные значения; нулевая цена разрешена.
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < decimal.Zero)
                throw new ArgumentOutOfRangeException(nameof(Price),
                    "Цена не может быть отрицательной.");
            _price = value;
        }
    }

    public int Year
    {
        get => _year;
        set
        {
            if (value < MinimumYear || value > DateTime.Today.Year + AllowedFutureYears)
                throw new ArgumentOutOfRangeException(nameof(Year),
                    "Год выпуска вне допустимого диапазона.");
            _year = value;
        }
    }

    // Конструктор без параметров вызывает конструктор с готовыми значениями.
    public ComputerEquipment()
        : this(Unspecified, Unspecified, decimal.Zero, DateTime.Today.Year)
    {
    }

    public ComputerEquipment(string brand, string model, decimal price, int year)
    {
        // Присваиваем через свойства: их проверки работают и при создании объекта.
        Brand = brand;
        Model = model;
        Price = price;
        Year = year;
    }

    // protected позволяет потомкам использовать ту же проверку непустых строк.
    protected static string ValidateRequiredText(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение не может быть пустым.", propertyName);
        return value.Trim();
    }

    protected string GetCommonInfo()
    {
        return $"Производитель: {Brand} | Модель: {Model} | "
            + $"Цена: {Price.ToString("0.00", CultureInfo.InvariantCulture)} руб. | "
            + $"Год: {Year}";
    }

    // virtual разрешает потомкам заменить этот метод своей реализацией.
    public virtual string GetInfo()
    {
        return $"Компьютерная техника | {GetCommonInfo()}";
    }

    // ToString тоже использует полиморфный GetInfo.
    public override string ToString()
    {
        return GetInfo();
    }
}
