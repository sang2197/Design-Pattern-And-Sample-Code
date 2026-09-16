namespace Exercises.Proxy.Example1;

// Proxy - Vi du 1: Virtual Proxy - tri hoan tai anh lon
// Xem lai: Proxy-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IImage (Subject) { void Display(); }
// - class RealImage : IImage (RealSubject)
//     public static int LoadCount { get; private set; }
//     constructor nhan string fileName -> goi LoadFromDisk() (private method tang LoadCount len 1)
//     Display() -> (khong can lam gi ca, de trong)
// - class ImageProxy : IImage (Proxy)
//     private readonly string _fileName; private RealImage? _realImage;
//     constructor nhan string fileName
//     Display() -> neu _realImage con null thi khoi tao _realImage = new RealImage(_fileName),
//       sau do goi _realImage.Display()
//       (LUU Y: chi khoi tao RealImage o LAN DAU, cac lan Display() sau khong duoc tao lai)
