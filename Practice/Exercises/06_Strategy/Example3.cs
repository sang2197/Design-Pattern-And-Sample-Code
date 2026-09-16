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
