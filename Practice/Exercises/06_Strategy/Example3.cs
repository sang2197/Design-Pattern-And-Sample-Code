namespace Exercises.Strategy.Example3;

// Strategy - Vi du 3: Chien luoc sap xep danh sach (tang dan / giam dan)
// Xem lai: Strategy-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau (can using System.Linq):
//
// - interface ISortStrategy (Strategy) { List<int> Sort(List<int> numbers); }
// - class AscendingSortStrategy : ISortStrategy -> Sort() -> numbers.OrderBy(x => x).ToList()
// - class DescendingSortStrategy : ISortStrategy -> Sort() -> numbers.OrderByDescending(x => x).ToList()
// - class NumberSorter (Context)
//     constructor nhan ISortStrategy strategy
//     SetStrategy(ISortStrategy) -> gan lai _strategy
//     Sort(numbers) -> uy quyen cho _strategy.Sort(numbers)

// Strategy
public interface IFileStorage
{
    string Save(string fileName);
}

// Concrete
public class LocalStorage : IFileStorage
{
    public string Save(string fileName) => $"Saved {fileName} to Local Storage";
}

public class S3Storage : IFileStorage
{
    public string Save(string fileName) => $"Saved {fileName} to Amazon S3";
}

// Context
public class FileContext
{
    private IFileStorage _strategy;
    public FileContext(IFileStorage strategy)
    {
        _strategy = strategy;
    }

    public string SaveFile(string fileName)
    {
        return _strategy.Save(fileName);
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var localStorage = new FileContext(new LocalStorage());
        Console.WriteLine(localStorage.SaveFile("file A"));

        var s3Storage = new FileContext(new S3Storage());
        Console.WriteLine(s3Storage.SaveFile("file B"));
    }
}