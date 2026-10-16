using System.Text;

namespace Lab2Variant9;

// ЧАСТЬ 1. Пароль хранит строку; операции создают новый объект.
public sealed class Password
{
    public const string DefaultValue = "Default1!";
    public const int MinimumLength = 6, MaximumLength = 12, StrongMinimumLength = 8;
    private readonly string _value;

    public string Value => _value;
    public int Length => _value.Length;

    public Password() : this(DefaultValue) { }

    public Password(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _value = value; // Не обрезаем пробелы: они являются частью введённой строки.
    }

    // Индексатор позволяет получить символ: password[0]. Нумерация с нуля.
    public char this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне строки.");
            return _value[index];
        }
    }

    public bool IsStrong()
    {
        if (Length < StrongMinimumLength || Length > MaximumLength) return false;
        bool upper = false, lower = false, digit = false, symbol = false;
        foreach (char item in _value)
        {
            if (char.IsWhiteSpace(item)) return false;
            if (char.IsUpper(item)) upper = true;
            else if (char.IsLower(item)) lower = true;
            else if (char.IsDigit(item)) digit = true;
            else if (!char.IsLetterOrDigit(item)) symbol = true;
        }
        return upper && lower && digit && symbol;
    }

    // По варианту минус заменяет последний символ, а не удаляет его.
    public static Password operator -(Password password, char item)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (password.Length == 0)
            throw new InvalidOperationException("В пустом пароле нечего заменять.");
        return new Password(password.Value.Substring(0, password.Length - 1) + item);
    }

    // Парные операции C# определяются вместе: >/<, !=/==, true/false.
    public static bool operator >(Password left, Password right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Length > right.Length;
    }

    public static bool operator <(Password left, Password right) => right > left;
    public static bool operator ==(Password? left, Password? right)
        => ReferenceEquals(left, right) || (left is not null && left.Equals(right));
    public static bool operator !=(Password? left, Password? right) => !(left == right);

    public static Password operator ++(Password password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return new Password(); // ++ сбрасывает значение на DefaultValue.
    }

    public static bool operator true(Password? password)
        => password is not null && password.IsStrong();
    public static bool operator false(Password? password)
        => password is null || !password.IsStrong();

    public override bool Equals(object? obj)
        => obj is Password other && Value == other.Value;
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => $"Password: значение={Value}, длина={Length}";
}

// Методы расширения вызываются как обычные методы строки и пароля.
public static class Extensions
{
    public static char MiddleCharacter(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length == 0)
            throw new InvalidOperationException("У пустой строки нет среднего символа.");
        // При чётной длине выбираем левый из двух средних символов.
        return text[(text.Length - 1) / 2];
    }

    public static bool HasValidLength(this Password password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return password.Length >= Password.MinimumLength &&
            password.Length <= Password.MaximumLength;
    }
}

// ЧАСТЬ 2. Эти данные создаются внутри Person: это композиция классов.
public sealed class PersonData
{
    public const int MaximumAge = 120;
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }

    public PersonData(int id, string name, int age)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (age < 0 || age > MaximumAge)
            throw new ArgumentOutOfRangeException(nameof(age));
        Id = id;
        Name = RequireText(name, nameof(name));
        Age = age;
    }

    public static string RequireText(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Текст не должен быть пустым.", parameter);
        return value.Trim();
    }

    public override string ToString() => $"PersonData: Id={Id}, имя={Name}, возраст={Age}";
}

// Абстрактную персону нельзя создать напрямую: нужен конкретный вид человека.
public abstract class Person
{
    private readonly PersonData _profile;
    public PersonData Profile => _profile;
    public int Id => _profile.Id;
    public string Name => _profile.Name;
    public int Age => _profile.Age;

    protected Person(int id, string name, int age)
    {
        _profile = new PersonData(id, name, age);
    }

    public abstract string GetRole();
    public virtual string DescribeActivity() => $"{Name}: деятельность не задана.";

    // Равенство персон определяется постоянным Id, независимо от роли.
    public override bool Equals(object? obj) => obj is Person other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString()
        => $"{GetType().Name}: Id={Id}, имя={Name}, возраст={Age}";

    // Учебная демонстрация Finalize: в C# он задаётся деструктором.
    // Ресурсов для освобождения нет; вывод здесь не делаем, время вызова задаёт GC.
    ~Person() { }
}

public class WorkingPerson : Person
{
    public string Workplace { get; }
    public WorkingPerson(int id, string name, int age, string workplace)
        : base(id, name, age)
        => Workplace = PersonData.RequireText(workplace, nameof(workplace));

    public override string GetRole() => "Работающий";
    public override string DescribeActivity() => $"{Name} работает в {Workplace}.";
    public override string ToString() => base.ToString() + $", место работы={Workplace}";
}

public class Employee : WorkingPerson
{
    public string Position { get; }
    public Employee(int id, string name, int age, string workplace, string position)
        : base(id, name, age, workplace)
        => Position = PersonData.RequireText(position, nameof(position));

    public override string GetRole() => "Служащий";
    public override string DescribeActivity() => $"{Name} выполняет обязанности: {Position}.";
    public override string ToString() => base.ToString() + $", должность={Position}";
}

// sealed делает класс «бесплодным»: от Turner нельзя наследоваться.
public sealed class Turner : WorkingPerson
{
    public int Grade { get; }
    public Turner(int id, string name, int age, string workplace, int grade)
        : base(id, name, age, workplace)
    {
        if (grade < 2 || grade > 6)
            throw new ArgumentOutOfRangeException(nameof(grade), "Разряд от 2 до 6.");
        Grade = grade;
    }

    public override string GetRole() => "Токарь";
    public override string DescribeActivity() => $"{Name} обрабатывает детали, разряд {Grade}.";
    public override string ToString() => base.ToString() + $", разряд={Grade}";
}

public class Programmer : Employee
{
    public string Language { get; }
    public Programmer(int id, string name, int age, string workplace, string language)
        : base(id, name, age, workplace, "Программист")
        => Language = PersonData.RequireText(language, nameof(language));

    public override string GetRole() => "Программист";
    public override string DescribeActivity() => $"{Name} пишет программы на {Language}.";
    public override string ToString() => base.ToString() + $", язык={Language}";
}

public class Learner : Person
{
    public string Institution { get; }
    public Learner(int id, string name, int age, string institution)
        : base(id, name, age)
        => Institution = PersonData.RequireText(institution, nameof(institution));

    public override string GetRole() => "Обучающийся";
    public override string DescribeActivity() => $"{Name} учится в {Institution}.";
    public override string ToString() => base.ToString() + $", место учёбы={Institution}";
}

public class Student : Learner
{
    public string Group { get; }
    public Student(int id, string name, int age, string institution, string group)
        : base(id, name, age, institution)
        => Group = PersonData.RequireText(group, nameof(group));

    public override string GetRole() => "Студент";
    public override string DescribeActivity() => $"{Name} посещает занятия группы {Group}.";
    public override string ToString() => base.ToString() + $", группа={Group}";
}

public sealed class PartTimeStudent : Student
{
    public string Workplace { get; }
    public PartTimeStudent(int id, string name, int age, string institution,
        string group, string workplace) : base(id, name, age, institution, group)
        => Workplace = PersonData.RequireText(workplace, nameof(workplace));

    public override string GetRole() => "Студент-заочник";
    public override string DescribeActivity()
        => $"{Name} учится заочно в {Institution} и работает в {Workplace}.";
    public override string ToString() => base.ToString() + $", место работы={Workplace}";
}

// Меню демонстрирует обе части. Команда --demo выводит готовый пример для отчёта.
public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Password password = new Password("Abcd123!");
        if (args.Contains("--demo"))
        {
            ShowPassword(password);
            DemonstrateOperators(password);
            DemonstrateExtensions(password, "abcde");
            ShowPeople(CreatePeople());
            DemonstrateObjectMethods();
            return;
        }

        while (true)
        {
            Console.WriteLine("\nЛабораторная 2, вариант 9");
            Console.WriteLine("1 — Задать пароль; 2 — Свойства пароля; 3 — Операции");
            Console.WriteLine("4 — Методы расширения; 5 — Полиморфизм; 6 — Методы Object");
            Console.Write("0 — Выход. Выбор: ");
            string? choice = Console.ReadLine();
            if (choice is null or "0") return;
            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Введите учебный пароль: ");
                        string? value = Console.ReadLine();
                        if (value is null) return;
                        password = new Password(value);
                        Console.WriteLine("Пароль сохранён.");
                        break;
                    case "2": ShowPassword(password); break;
                    case "3": DemonstrateOperators(password); break;
                    case "4":
                        Console.Write("Введите строку: ");
                        string? text = Console.ReadLine();
                        if (text is null) return;
                        DemonstrateExtensions(password, text);
                        break;
                    case "5": ShowPeople(CreatePeople()); break;
                    case "6": DemonstrateObjectMethods(); break;
                    default: Console.WriteLine("Выберите пункт от 0 до 6."); break;
                }
            }
            catch (Exception error) when (error is ArgumentException or InvalidOperationException)
            {
                Console.WriteLine("Ошибка: " + error.Message);
            }
        }
    }

    private static void ShowPassword(Password password)
    {
        Console.WriteLine("\nЧасть 1. " + password);
        Console.WriteLine($"Допустимая длина 6–12: {password.HasValidLength()}");
        if (password) Console.WriteLine("operator true: пароль стойкий.");
        else Console.WriteLine("operator true: пароль слабый.");
        if (password.Length > 0) Console.WriteLine($"Индексатор [0]: {password[0]}");
    }

    private static void DemonstrateOperators(Password password)
    {
        Password other = new Password("abc123");
        Password copy = new Password(password.Value);
        Console.WriteLine($"Сравнение с {other.Value}: > {password > other}, < {password < other}");
        Console.WriteLine($"Сравнение с копией: == {password == copy}, != {password != copy}");
        if (password.Length > 0)
            Console.WriteLine($"Замена последнего символа (- '#'): {(password - '#').Value}");
        else Console.WriteLine("Замена: пустой пароль, символа нет.");
        copy++;
        Console.WriteLine($"Сброс копии (++): {copy.Value}; исходный пароль: {password.Value}");
    }

    private static void DemonstrateExtensions(Password password, string text)
    {
        Console.WriteLine($"Длина пароля 6–12: {password.HasValidLength()}");
        Console.WriteLine($"Средний символ строки «{text}»: {text.MiddleCharacter()}");
    }

    public static Person[] CreatePeople()
    {
        // Все ссылки имеют тип Person, но объекты принадлежат семи разным классам.
        return new Person[]
        {
            new WorkingPerson(1, "Иван", 35, "Завод"),
            new Employee(2, "Анна", 29, "Банк", "Менеджер"),
            new Turner(3, "Павел", 42, "Завод", 5),
            new Programmer(4, "Олег", 27, "ИТ-компания", "C#"),
            new Learner(5, "Мария", 17, "Колледж"),
            new Student(6, "Алексей", 19, "НГТУ", "АВТ-513"),
            new PartTimeStudent(7, "Денис", 24, "НГТУ", "ЗА-21", "Сервис")
        };
    }

    private static void ShowPeople(Person[] people)
    {
        Console.WriteLine("\nЧасть 2. Массив Person[] и виртуальные вызовы");
        foreach (Person person in people)
        {
            Console.WriteLine("Роль: " + person.GetRole());
            Console.WriteLine(person.DescribeActivity());
            Console.WriteLine(person.ToString());
        }
    }

    private static void DemonstrateObjectMethods()
    {
        Person first = new Student(10, "Алексей", 19, "НГТУ", "АВТ-513");
        Person same = new Programmer(10, "Алексей", 19, "Сервис", "C#");
        Person other = new Student(11, "Алексей", 19, "НГТУ", "АВТ-513");
        Console.WriteLine("\nObject: равенство персон по Id");
        Console.WriteLine($"Одинаковый Id: Equals={first.Equals(same)}");
        Console.WriteLine($"Разный Id: Equals={first.Equals(other)}");
        Console.WriteLine($"Хеши одинакового Id: {first.GetHashCode()}, {same.GetHashCode()}");
        Console.WriteLine(first.ToString());
        Console.WriteLine("Finalize задан деструктором Person; его вызов планирует GC.");
        Console.WriteLine("GetType и MemberwiseClone не виртуальные, override для них невозможен.");
    }
}
