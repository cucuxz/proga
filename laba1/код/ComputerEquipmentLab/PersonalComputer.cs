namespace ComputerEquipmentLab;

// После двоеточия указан родитель: ПК наследует общие свойства техники.
public class PersonalComputer : ComputerEquipment
{
    public const int MinimumRamGb = 1;

    private string _processor = string.Empty;
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
                throw new ArgumentOutOfRangeException(nameof(RamGb),
                    "Объем памяти должен быть положительным.");
            _ramGb = value;
        }
    }

    // Автосвойство: допустимы оба значения bool, отдельная проверка не нужна.
    public bool HasDiscreteGpu { get; set; }

    public PersonalComputer()
        : this(Unspecified, Unspecified, decimal.Zero, DateTime.Today.Year,
               Unspecified, MinimumRamGb, false)
    {
    }

    // base(...) заполняет общие свойства, тело конструктора — свойства ПК.
    public PersonalComputer(string brand, string model, decimal price, int year,
                            string processor, int ramGb, bool hasDiscreteGpu)
        : base(brand, model, price, year)
    {
        Processor = processor;
        RamGb = ramGb;
        HasDiscreteGpu = hasDiscreteGpu;
    }

    // override заменяет базовый метод и добавляет характеристики ПК.
    public override string GetInfo()
    {
        string gpu = HasDiscreteGpu ? "да" : "нет";
        return $"Персональный компьютер | {GetCommonInfo()} | "
            + $"Процессор: {Processor} | ОЗУ: {RamGb} ГБ | "
            + $"Дискретная видеокарта: {gpu}";
    }
}
