namespace Exercises.Proxy;

public interface IImage
{
    void Display();
}

public class RealImage : IImage
{
    private readonly string _fileName;
    public static int LoadCount { get; private set; }
    public List<string> DisplayLog { get; } = new List<string>();

    public RealImage(string fileName)
    {
        _fileName = fileName;
        LoadFromDisk();
    }

    private void LoadFromDisk()
    {
        LoadCount++;
    }

    public void Display()
    {
        DisplayLog.Add($"Hien thi anh {_fileName}");
    }
}

public class ImageProxy : IImage
{
    private readonly string _fileName;
    private RealImage? _realImage;

    public ImageProxy(string fileName)
    {
        _fileName = fileName;
    }

    // TODO: chi khoi tao _realImage = new RealImage(_fileName) o LAN DAU (khi con null),
    // sau do goi _realImage.Display()
    public void Display()
    {
        throw new NotImplementedException();
    }
}
