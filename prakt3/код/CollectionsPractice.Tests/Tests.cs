using System.Collections;
using CollectionsPractice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Независимые проверки сравнений, проверок свойств и глубокой копии.
[TestClass]
public class EquipmentTests
{
    private static ComputerEquipment Item(string brand = "Acer", decimal price = 100)
        => new ComputerEquipment(brand, "One", price, 2024);

    [TestMethod]
    public void CompareToOrdersByPrice()
    {
        Assert.IsTrue(Item(price: 100).CompareTo(Item(price: 200)) < 0);
        Assert.IsTrue(Item(price: 200).CompareTo(Item(price: 100)) > 0);
        Assert.AreEqual(0, Item(price: 100).CompareTo(Item(price: 100)));
    }

    [TestMethod]
    public void CompareToRejectsWrongType()
    {
        Assert.AreEqual(1, Item().CompareTo(null));
        Assert.ThrowsExactly<ArgumentException>(() => Item().CompareTo("Acer"));
    }

    [TestMethod]
    public void ExternalComparerOrdersByName()
    {
        EquipmentComparer comparer = new EquipmentComparer();
        Assert.IsTrue(comparer.Compare(Item("Acer", 200), Item("Lenovo", 100)) < 0);
        Assert.AreEqual(0, comparer.Compare(Item("acer"), Item("Acer")));
        Assert.IsTrue(comparer.Compare(new ComputerEquipment("Acer", "A", 1, 2024),
            new ComputerEquipment("Acer", "B", 1, 2024)) < 0);
    }

    [TestMethod]
    public void NonGenericComparerWorksAndRejectsWrongType()
    {
        IComparer comparer = new EquipmentComparer();
        Assert.IsTrue(comparer.Compare(Item("Acer"), Item("Lenovo")) < 0);
        Assert.ThrowsExactly<ArgumentException>(() => comparer.Compare("Acer", Item()));
    }

    [TestMethod]
    public void ComparerHandlesNulls()
    {
        EquipmentComparer comparer = new EquipmentComparer();
        Assert.AreEqual(0, comparer.Compare(null, null));
        Assert.IsTrue(comparer.Compare(null, Item()) < 0);
        Assert.IsTrue(comparer.Compare(Item(), null) > 0);
    }

    [TestMethod]
    public void CloneCopiesValuesAndCreatesSeparateObject()
    {
        ComputerEquipment source = Item();
        source.Accessories.Add("Мышь");
        ComputerEquipment copy = (ComputerEquipment)source.Clone();
        Assert.AreNotSame(source, copy);
        Assert.AreEqual(source.Brand, copy.Brand);
        Assert.AreEqual(source.Model, copy.Model);
        Assert.AreEqual(source.Price, copy.Price);
        Assert.AreEqual(source.Year, copy.Year);
        CollectionAssert.AreEqual(source.Accessories, copy.Accessories);
    }

    [TestMethod]
    public void CloneOwnsIndependentNestedList()
    {
        ComputerEquipment source = Item();
        source.Accessories.Add("Мышь");
        ComputerEquipment copy = (ComputerEquipment)source.Clone();
        copy.Brand = "Копия";
        copy.Accessories[0] = "Клавиатура";
        copy.Accessories.Add("Кабель");
        Assert.AreNotSame(source.Accessories, copy.Accessories);
        Assert.AreEqual("Acer", source.Brand);
        CollectionAssert.AreEqual(new[] { "Мышь" }, source.Accessories);
        Assert.AreEqual(2, copy.Accessories.Count);
    }

    [TestMethod]
    public void InvalidPropertiesAreRejected()
    {
        Assert.ThrowsExactly<ArgumentException>(() => Item(" "));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Item(price: -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new ComputerEquipment("A", "B", 1, 1899));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new ComputerEquipment("A", "B", 1, DateTime.Today.Year + 2));
        ComputerEquipment item = Item(" Acer ");
        Assert.AreEqual("Acer", item.Brand);
        Assert.ThrowsExactly<ArgumentException>(() => item.Model = "");
    }
}

[TestClass]
public class CollectionTests
{
    private static string Capture(Action action)
    {
        TextWriter original = Console.Out;
        using StringWriter text = new StringWriter();
        try
        {
            Console.SetOut(text);
            action();
            return text.ToString();
        }
        finally { Console.SetOut(original); }
    }

    [TestMethod]
    public void ArrayListHasFiveNumbersAndString()
    {
        ArrayList items = Program.CreateArrayList(new Random(1));
        Assert.AreEqual(6, items.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.IsInstanceOfType<int>(items[i]);
            Assert.IsTrue((int)items[i]! >= 1 && (int)items[i]! < 100);
        }
        Assert.AreEqual("C#", items[5]);
    }

    [TestMethod]
    public void ArrayListDemoRemovesAndFinds()
    {
        string text = Capture(() => Program.ShowArrayList(true));
        StringAssert.Contains(text, "удалён");
        StringAssert.Contains(text, "количество = 5");
        StringAssert.Contains(text, "Поиск C#: True");
    }

    [TestMethod]
    public void RemoveTopPreservesLifoOrder()
    {
        Stack<char> stack = new Stack<char>(new[] { 'A', 'B', 'C', 'D', 'E' });
        Program.RemoveTop(stack, 2);
        CollectionAssert.AreEqual(new[] { 'C', 'B', 'A' }, stack.ToArray());
    }

    [TestMethod]
    public void RemoveTopHandlesZeroAndAll()
    {
        Stack<char> stack = new Stack<char>(new[] { 'A', 'B' });
        Program.RemoveTop(stack, 0);
        Assert.AreEqual(2, stack.Count);
        Program.RemoveTop(stack, 2);
        Assert.AreEqual(0, stack.Count);
        Program.RemoveTop(stack, 0);
    }

    [TestMethod]
    public void InvalidRemovalDoesNotChangeStack()
    {
        Stack<char> stack = new Stack<char>(new[] { 'A', 'B' });
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Program.RemoveTop(stack, -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Program.RemoveTop(stack, 3));
        CollectionAssert.AreEqual(new[] { 'B', 'A' }, stack.ToArray());
    }

    [TestMethod]
    public void CharacterDemoShowsFinalListAndSearch()
    {
        string text = Capture(() => Program.ShowCharacters(true)).Replace("\r", "");
        StringAssert.Contains(text, "Вторая коллекция List<char>:\n  Y\n  X\n  C\n  B\n  A");
        StringAssert.Contains(text, "Поиск X в списке: True");
    }

    [TestMethod]
    public void EquipmentStackAndSearchUseModels()
    {
        Stack<ComputerEquipment> stack = Program.CreateEquipmentStack();
        Assert.AreEqual("Aspire", stack.Peek().Model);
        ComputerEquipment? found = Program.FindByModel(stack, "optiplex");
        Assert.IsNotNull(found);
        Assert.AreEqual("Dell", found.Brand);
        Assert.IsNull(Program.FindByModel(stack, "Missing"));
    }

    [TestMethod]
    public void ListSortUsesBothInterfaces()
    {
        List<ComputerEquipment> list = new List<ComputerEquipment>(Program.CreateEquipmentStack());
        list.Sort();
        CollectionAssert.AreEqual(new[] { "Dell", "Acer", "Lenovo" }, list.Select(x => x.Brand).ToArray());
        list.Sort(new EquipmentComparer());
        CollectionAssert.AreEqual(new[] { "Acer", "Dell", "Lenovo" }, list.Select(x => x.Brand).ToArray());
    }

    [TestMethod]
    public void EquipmentDemoShowsIndependentClone()
    {
        string text = Capture(() => Program.ShowEquipment(true));
        StringAssert.Contains(text, "Поиск OptiPlex: Dell OptiPlex");
        StringAssert.Contains(text, "Оригинал: Asus Zenbook");
        StringAssert.Contains(text, "Копия:    Asus Копия");
        StringAssert.Contains(text, "Общий объект: False");
        StringAssert.Contains(text, "Общий список комплекта: False");
    }

    [TestMethod]
    public void ObservableDemoInvokesRegisteredHandler()
    {
        string text = Capture(Program.ShowObservable);
        Assert.AreEqual(2, text.Split("Событие CollectionChanged: Add").Length - 1);
        Assert.AreEqual(1, text.Split("Событие CollectionChanged: Remove").Length - 1);
        StringAssert.Contains(text, "Удалены:");
        StringAssert.Contains(text, "Оставшиеся объекты:\n  Acer Aspire");
    }
}
