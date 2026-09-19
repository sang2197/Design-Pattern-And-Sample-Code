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

public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _intance = new Lazy<AppSettings>(() => new AppSettings());

    public string ConnectionString { get; }
    private AppSettings()
    {
        ConnectionString = "Server=localhost;Database=MyApp";
        Console.WriteLine("[AppSetting] Da doc cau hinh tu code");
    }

    public static AppSettings Intance => _intance.Value;
}

public class Program
{
    public static void Main()
    {
        var setting = AppSettings.Intance;
        Console.WriteLine(setting.ConnectionString);
    }
}