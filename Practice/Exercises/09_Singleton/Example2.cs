namespace Exercises.Singleton.Example2;

// Singleton - Vi du 2: Bo dem dung chung tren toan ung dung
// Xem lai: Singleton-Pattern.md - Vi du 2
//
// Viet lai TU DAU:
//
// - sealed class RequestCounter
//     private static readonly Lazy<RequestCounter> _instance = new Lazy<RequestCounter>(() => new RequestCounter());
//     private int _count;
//     private constructor() (rong)
//     public static RequestCounter Instance => _instance.Value
//     Increment() -> tang _count len 1
//     GetCount() -> tra ve _count
