namespace Exercises.Singleton.Example3;

// Singleton - Vi du 3: Logger dung chung cho nhieu service khac nhau
// Xem lai: Singleton-Pattern.md - Vi du 3
//
// Viet lai TU DAU:
//
// - sealed class AppLogger
//     private static readonly Lazy<AppLogger> _instance = new Lazy<AppLogger>(() => new AppLogger());
//     private readonly List<string> _logs = new List<string>();
//     private constructor() (rong)
//     public static AppLogger Instance => _instance.Value
//     Log(message) -> them message vao _logs
//     int LogCount => _logs.Count
// - class OrderService -> CreateOrder(orderId) goi AppLogger.Instance.Log("Tao don hang {orderId}")
// - class PaymentService -> Pay(orderId) goi AppLogger.Instance.Log("Thanh toan don hang {orderId}")
