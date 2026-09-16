namespace Exercises.Singleton;

public sealed class AppSettings
{
    // TODO: khai bao 1 static readonly Lazy<AppSettings> de tri hoan khoi tao
    // vi du: private static readonly Lazy<AppSettings> _instance = new Lazy<AppSettings>(() => new AppSettings());

    public string ConnectionString { get; }
    public int InitCount { get; private set; }

    // Constructor phai la private - ngan ben ngoai tu "new" them instance khac
    private AppSettings()
    {
        ConnectionString = "Server=localhost;Database=MyApp;";
        InitCount++;
    }

    // TODO: expose static property Instance, tra ve _instance.Value
    public static AppSettings Instance => throw new NotImplementedException();
}
