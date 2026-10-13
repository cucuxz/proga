using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputerEquipmentLab;

[assembly: DoNotParallelize]

namespace ComputerEquipmentLab.Tests;

// MSTest сам находит и запускает методы с атрибутом TestMethod.
[TestClass]
public class ComputerEquipmentTests
{
    // Значения, заданные конструктором без параметров.
    [TestMethod]
    public void BaseDefaults()
    {
        ComputerEquipment item = new();
        Assert.AreEqual(ComputerEquipment.Unspecified, item.Brand);
        Assert.AreEqual(ComputerEquipment.Unspecified, item.Model);
        Assert.AreEqual(decimal.Zero, item.Price);
        Assert.AreEqual(DateTime.Today.Year, item.Year);
    }

    // Переданные данные, удаление пробелов и формат цены.
    [TestMethod]
    public void BaseParameterized()
    {
        ComputerEquipment item = new("  Acme  ", "Tower", 45000.50m, 2024);
        Assert.AreEqual("Acme", item.Brand);
        Assert.AreEqual("Tower", item.Model);
        Assert.AreEqual(45000.50m, item.Price);
        Assert.AreEqual(2024, item.Year);
        StringAssert.Contains(item.GetInfo(), "Компьютерная техника");
        StringAssert.Contains(item.GetInfo(), "45000.50 руб.");
    }

    // Изменение свойств уже созданного объекта.
    [TestMethod]
    public void BasePropertiesCanChange()
    {
        ComputerEquipment item = new();
        item.Brand = "  Lenovo ";
        item.Model = "ThinkCentre";
        item.Price = 0m;
        item.Year = ComputerEquipment.MinimumYear;
        Assert.AreEqual("Lenovo", item.Brand);
        Assert.AreEqual("ThinkCentre", item.Model);
        Assert.AreEqual(0m, item.Price);
        Assert.AreEqual(ComputerEquipment.MinimumYear, item.Year);
    }

    // Пустая строка и null должны вызывать исключение.
    [TestMethod]
    public void EmptyTextRejected()
    {
        ComputerEquipment item = new();
        Assert.ThrowsExactly<ArgumentException>(() => item.Brand = "   ");
        Assert.ThrowsExactly<ArgumentException>(() => item.Model = null!);
        Assert.AreEqual(ComputerEquipment.Unspecified, item.Brand);
    }

    // Отрицательная цена запрещена; прежняя цена сохраняется.
    [TestMethod]
    public void NegativePriceRejected()
    {
        ComputerEquipment item = new();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => item.Price = -0.01m);
        Assert.AreEqual(decimal.Zero, item.Price);
    }

    // Обе границы года разрешены, значения за границами запрещены.
    [TestMethod]
    public void YearBoundaries()
    {
        ComputerEquipment item = new();
        item.Year = ComputerEquipment.MinimumYear;
        item.Year = DateTime.Today.Year + ComputerEquipment.AllowedFutureYears;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => item.Year = ComputerEquipment.MinimumYear - 1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => item.Year = DateTime.Today.Year + ComputerEquipment.AllowedFutureYears + 1);
    }

    // Начальные параметры ПК.
    [TestMethod]
    public void PersonalComputerDefaults()
    {
        PersonalComputer item = new();
        Assert.AreEqual(PersonalComputer.MinimumRamGb, item.RamGb);
        Assert.AreEqual(ComputerEquipment.Unspecified, item.Processor);
        Assert.AreEqual(false, item.HasDiscreteGpu);
    }

    // Изменение памяти и видеокарты отражается в GetInfo.
    [TestMethod]
    public void PersonalComputerProperties()
    {
        PersonalComputer item = new("Dell", "OptiPlex", 60000m, 2025, "  Core i5  ", 16, true);
        item.RamGb = 32;
        item.HasDiscreteGpu = false;
        Assert.AreEqual("Core i5", item.Processor);
        Assert.AreEqual(32, item.RamGb);
        StringAssert.Contains(item.GetInfo(), "Персональный компьютер");
        StringAssert.Contains(item.GetInfo(), "Дискретная видеокарта: нет");
    }

    // Пустое название процессора ПК запрещено.
    [TestMethod]
    public void InvalidPersonalComputerProcessor() =>
        Assert.ThrowsExactly<ArgumentException>(() => new PersonalComputer("Dell", "X", 1m, 2024, " ", 8, false));

    // Нулевой объём памяти ПК запрещён.
    [TestMethod]
    public void InvalidPersonalComputerRam() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PersonalComputer("Dell", "X", 1m, 2024, "CPU", 0, false));

    // Начальные параметры ноутбука.
    [TestMethod]
    public void LaptopDefaults()
    {
        Laptop item = new();
        Assert.AreEqual(Laptop.MinimumRamGb, item.RamGb);
        Assert.AreEqual(Laptop.DefaultScreenSizeInches, item.ScreenSizeInches);
        Assert.AreEqual(Laptop.MinimumBatteryCapacityMah, item.BatteryCapacityMah);
    }

    // Изменение экрана и аккумулятора отражается в свойствах и выводе.
    [TestMethod]
    public void LaptopProperties()
    {
        Laptop item = new("Asus", "Zenbook", 90000m, 2025, "Ryzen 7", 16, 14.5m, 6000);
        item.ScreenSizeInches = 15.6m;
        item.BatteryCapacityMah = 7000;
        Assert.AreEqual("Ryzen 7", item.Processor);
        Assert.AreEqual(16, item.RamGb);
        Assert.AreEqual(15.6m, item.ScreenSizeInches);
        Assert.AreEqual(7000, item.BatteryCapacityMah);
        StringAssert.Contains(item.GetInfo(), "Ноутбук");
        StringAssert.Contains(item.GetInfo(), "Экран: 15.6 дюйм.");
    }

    // Пустое название процессора ноутбука запрещено.
    [TestMethod]
    public void InvalidLaptopProcessor() =>
        Assert.ThrowsExactly<ArgumentException>(() => new Laptop("Asus", "X", 1m, 2024, "", 8, 14m, 5000));

    // Нулевой объём памяти ноутбука запрещён.
    [TestMethod]
    public void InvalidLaptopRam() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 0, 14m, 5000));

    // Нулевая диагональ запрещена.
    [TestMethod]
    public void InvalidScreenSize() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 8, 0m, 5000));

    // Нулевая ёмкость аккумулятора запрещена.
    [TestMethod]
    public void InvalidBatteryCapacity() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Laptop("Asus", "X", 1m, 2024, "CPU", 8, 14m, 0));

    // Базовая ссылка вызывает вывод фактического типа объекта.
    [TestMethod]
    public void PolymorphismAndToString()
    {
        List<ComputerEquipment> items =
        [
            new ComputerEquipment("A", "B", 0m, 2024),
            new PersonalComputer("C", "D", 1m, 2024, "CPU", 8, true),
            new Laptop("E", "F", 2m, 2024, "CPU", 16, 15m, 5000)
        ];
        StringAssert.Contains(items[0].GetInfo(), "Компьютерная техника");
        StringAssert.Contains(items[1].GetInfo(), "Персональный компьютер");
        StringAssert.Contains(items[2].GetInfo(), "Ноутбук");
        foreach (ComputerEquipment item in items)
            Assert.AreEqual(item.GetInfo(), item.ToString());
    }

    // Удаление ПК сохраняет порядок оставшихся устройств.
    [TestMethod]
    public void OrderedListAfterRemoval()
    {
        List<ComputerEquipment> items = [new ComputerEquipment(), new PersonalComputer(), new Laptop()];
        items.RemoveAt(1);
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(typeof(ComputerEquipment), items[0].GetType());
        Assert.AreEqual(typeof(Laptop), items[1].GetType());
    }
}
