namespace Exercises.Singleton.Example1;

// Singleton - Vi du 1: Doc cau hinh ung dung mot lan duy nhat
// Xem lai: Singleton-Pattern.md - Vi du 1
//
// Viet lai TU DAU:
//
// - sealed class AppSettings
//     private static readonly Lazy<AppSettings> _instance = new Lazy<AppSettings>(() => new AppSettings());
//     string ConnectionString { get; }; int InitCount { get; private set; }
//     private constructor() -> gan ConnectionString = "Server=localhost;Database=MyApp;", tang InitCount len 1
//       (constructor PHAI la private de ngan ben ngoai "new" them instance khac)
//     public static AppSettings Instance => _instance.Value
