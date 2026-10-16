using Lab2Variant9;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab2Variant9.Tests;

// Каждый TestMethod независимо создаёт данные и сравнивает результат с ожидаемым.
[TestClass]
public class PasswordTests
{
    [TestMethod]
    public void DefaultConstructorSetsDefaultValue()
        => Assert.AreEqual("Default1!", new Password().Value);

    [TestMethod]
    public void ConstructorPreservesExactText()
    {
        Password value = new Password(" Abc1! ");
        Assert.AreEqual(" Abc1! ", value.Value);
        Assert.AreEqual(7, value.Length);
    }

    [TestMethod]
    public void NullValueRejected()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new Password(null!));

    [TestMethod]
    public void IndexerReadsCharacter()
    {
        Password value = new Password("Abcd123!");
        Assert.AreEqual('A', value[0]);
        Assert.AreEqual('!', value[7]);
    }

    [TestMethod]
    public void IndexerRejectsNegativeAndEnd()
    {
        Password value = new Password("abc");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _ = value[-1]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _ = value[3]);
    }

    [TestMethod]
    public void ReplacementChangesLastCharacterOnly()
    {
        Assert.AreEqual("Abcd123#", (new Password("Abcd123!") - '#').Value);
        Assert.AreEqual("#", (new Password("A") - '#').Value);
    }

    [TestMethod]
    public void ReplacementDoesNotChangeSource()
    {
        Password source = new Password("abc!");
        Password result = source - '#';
        Assert.AreEqual("abc!", source.Value);
        Assert.AreEqual("abc#", result.Value);
        Assert.AreNotSame(source, result);
    }

    [TestMethod]
    public void ReplacementOnEmptyRejected()
        => Assert.ThrowsExactly<InvalidOperationException>(() => _ = new Password("") - '#');

    [TestMethod]
    public void LongerPasswordComparison()
    {
        Password longer = new Password("abcd"), shorter = new Password("ab");
        Assert.IsTrue(longer > shorter);
        Assert.IsTrue(shorter < longer);
        Assert.IsFalse(shorter > longer);
    }

    [TestMethod]
    public void EqualLengthsNeitherGreaterNorLess()
    {
        Password first = new Password("abc"), second = new Password("xyz");
        Assert.IsFalse(first > second);
        Assert.IsFalse(first < second);
    }

    [TestMethod]
    public void EqualityUsesText()
    {
        Password first = new Password("Abc!"), same = new Password("Abc!");
        Assert.IsTrue(first == same);
        Assert.IsFalse(first != same);
        Assert.IsTrue(first != new Password("abc!"));
        Assert.IsTrue(first.Equals(same));
        Assert.IsFalse(first.Equals("Abc!"));
    }

    [TestMethod]
    public void EqualityHandlesNull()
    {
        Password? missing = null;
        Password value = new Password();
        Assert.IsTrue(missing == null);
        Assert.IsFalse(value == missing);
        Assert.IsFalse(missing == value);
        Assert.IsTrue(value != missing);
        Assert.IsFalse(value.Equals(null));
    }

    [TestMethod]
    public void HashCodeMatchesEquality()
        => Assert.AreEqual(new Password("abc").GetHashCode(),
            new Password("abc").GetHashCode());

    [TestMethod]
    public void PrefixResetUsesDefault()
    {
        Password value = new Password("Other1!");
        Password result = ++value;
        Assert.AreEqual("Default1!", value.Value);
        Assert.AreSame(value, result);
    }

    [TestMethod]
    public void PostfixResetRetainsOldValue()
    {
        Password value = new Password("Other1!");
        Password old = value++;
        Assert.AreEqual("Other1!", old.Value);
        Assert.AreEqual("Default1!", value.Value);
        Assert.AreNotSame(old, value);
    }

    [TestMethod]
    public void StrongPasswordTrueOperator()
    {
        bool strong;
        if (new Password("Abcd123!")) strong = true;
        else strong = false;
        Assert.IsTrue(strong);
        Password? missing = null;
        bool nullStrong;
        if (missing) nullStrong = true;
        else nullStrong = false;
        Assert.IsFalse(nullStrong);
    }

    [TestMethod]
    public void MissingStrengthCategoryRejected()
    {
        foreach (string value in new[] { "abcd123!", "ABCD123!", "Abcdefg!", "Abcd1234" })
            Assert.IsFalse(new Password(value).IsStrong(), value);
    }

    [TestMethod]
    public void WhitespaceMakesPasswordWeak()
        => Assert.IsFalse(new Password("Abcd1! x").IsStrong());

    [TestMethod]
    public void StrengthBoundaries()
    {
        Assert.IsFalse(new Password("Abc123!").IsStrong()); // 7 символов.
        Assert.IsTrue(new Password("Abcd123!").IsStrong()); // 8 символов.
        Assert.IsTrue(new Password("Abcd1234567!").IsStrong()); // 12 символов.
        Assert.IsFalse(new Password("Abcd12345678!").IsStrong()); // 13 символов.
    }

    [TestMethod]
    public void LengthExtensionBoundaries()
    {
        Assert.IsFalse(new Password("12345").HasValidLength());
        Assert.IsTrue(new Password("123456").HasValidLength());
        Assert.IsTrue(new Password("123456789012").HasValidLength());
        Assert.IsFalse(new Password("1234567890123").HasValidLength());
        Assert.IsFalse(new Password("").HasValidLength());
    }

    [TestMethod]
    public void MiddleCharacterOddAndEven()
    {
        Assert.AreEqual('c', "abcde".MiddleCharacter());
        Assert.AreEqual('b', "abcd".MiddleCharacter());
        Assert.AreEqual('Я', "Я".MiddleCharacter());
    }

    [TestMethod]
    public void MiddleCharacterEmptyRejected()
        => Assert.ThrowsExactly<InvalidOperationException>(() => "".MiddleCharacter());

    [TestMethod]
    public void ExtensionNullRejected()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Extensions.MiddleCharacter(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => Extensions.HasValidLength(null!));
    }
}

[TestClass]
public class PersonTests
{
    [TestMethod]
    public void PersonDataValidatesIdNameAge()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PersonData(0, "Иван", 20));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PersonData(1, "Иван", -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PersonData(1, "Иван", 121));
        Assert.ThrowsExactly<ArgumentException>(() => new PersonData(1, " ", 20));
        Assert.AreEqual(120, new PersonData(1, "Иван", 120).Age);
    }

    [TestMethod]
    public void ConstructorsPreservePersonProperties()
    {
        Student student = new Student(10, " Алексей ", 19, "НГТУ", "АВТ-513");
        Assert.AreEqual(10, student.Id);
        Assert.AreEqual("Алексей", student.Name);
        Assert.AreEqual(19, student.Age);
        Assert.AreEqual("НГТУ", student.Institution);
        Assert.AreEqual("АВТ-513", student.Group);
    }

    [TestMethod]
    public void ArrayContainsAllSevenRoles()
    {
        Person[] people = Program.CreatePeople();
        string[] actual = people.Select(person => person.GetRole()).ToArray();
        CollectionAssert.AreEquivalent(new[] { "Работающий", "Служащий", "Токарь",
            "Программист", "Обучающийся", "Студент", "Студент-заочник" }, actual);
        Assert.AreEqual(7, people.Select(person => person.GetType()).Distinct().Count());
    }

    [TestMethod]
    public void VirtualActivityUsesActualType()
    {
        Person worker = new Turner(1, "Павел", 42, "Завод", 5);
        Person programmer = new Programmer(2, "Олег", 27, "Сервис", "C#");
        Person student = new PartTimeStudent(3, "Денис", 24, "НГТУ", "ЗА-21", "Сервис");
        Assert.AreEqual("Павел обрабатывает детали, разряд 5.", worker.DescribeActivity());
        Assert.AreEqual("Олег пишет программы на C#.", programmer.DescribeActivity());
        Assert.AreEqual("Денис учится заочно в НГТУ и работает в Сервис.", student.DescribeActivity());
    }

    [TestMethod]
    public void EveryToStringShowsTypeAndOwnProperties()
    {
        Person[] people = Program.CreatePeople();
        string[] details = { "место работы=Завод", "должность=Менеджер", "разряд=5",
            "язык=C#", "место учёбы=Колледж", "группа=АВТ-513", "место работы=Сервис" };
        for (int i = 0; i < people.Length; i++)
        {
            string text = people[i].ToString();
            StringAssert.Contains(text, people[i].GetType().Name);
            StringAssert.Contains(text, "имя=" + people[i].Name);
            StringAssert.Contains(text, "возраст=" + people[i].Age);
            StringAssert.Contains(text, details[i]);
        }
    }

    [TestMethod]
    public void PersonEqualityUsesIdentifier()
    {
        Person student = new Student(10, "Иван", 20, "НГТУ", "АВТ-513");
        Person programmer = new Programmer(10, "Иван", 20, "Сервис", "C#");
        Person other = new Student(11, "Иван", 20, "НГТУ", "АВТ-513");
        Assert.IsTrue(student.Equals(programmer));
        Assert.IsTrue(programmer.Equals(student));
        Assert.IsFalse(student.Equals(other));
        Assert.IsFalse(student.Equals(null));
        Assert.IsFalse(student.Equals("Иван"));
    }

    [TestMethod]
    public void PersonHashCodeStableByIdentifier()
    {
        Person first = new Learner(10, "Иван", 20, "НГТУ");
        Person second = new WorkingPerson(10, "Иван", 20, "Сервис");
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        Assert.AreEqual(1, new HashSet<Person> { first, second }.Count);
    }

    [TestMethod]
    public void InvalidSpecialPropertiesRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Turner(1, "Иван", 20, "Завод", 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Turner(1, "Иван", 20, "Завод", 7));
        Assert.ThrowsExactly<ArgumentException>(() => new Programmer(1, "Иван", 20, "Сервис", " "));
        Assert.ThrowsExactly<ArgumentException>(() => new Student(1, "Иван", 20, "НГТУ", ""));
        Assert.AreEqual(2, new Turner(1, "Иван", 20, "Завод", 2).Grade);
        Assert.AreEqual(6, new Turner(1, "Иван", 20, "Завод", 6).Grade);
    }

    [TestMethod]
    public void PersonProfilesNotShared()
    {
        Person first = new Learner(1, "Иван", 20, "НГТУ");
        Person second = new Learner(1, "Иван", 20, "НГТУ");
        Assert.AreNotSame(first.Profile, second.Profile);
        Assert.AreEqual(first.Name, first.Profile.Name);
        Assert.AreEqual(first.Age, first.Profile.Age);
    }
}
