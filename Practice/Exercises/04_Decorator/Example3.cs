namespace Exercises.Decorator.Example3;

// Decorator - Vi du 3: Ghep pipeline xu ly du lieu (nen roi ma hoa)
// Xem lai: Decorator-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IDataSource (Component) { string Write(string data); }
// - class FileDataSource : IDataSource (ConcreteComponent) -> Write() tra ve nguyen data
// - abstract class DataSourceDecorator : IDataSource (Decorator)
//     protected readonly IDataSource Inner; constructor nhan IDataSource inner
//     virtual Write(data) => Inner.Write(data)
// - class CompressionDecorator : DataSourceDecorator (ConcreteDecorator)
//     Write(data) -> "[compressed]{Inner.Write(data)}"
// - class EncryptionDecorator : DataSourceDecorator (ConcreteDecorator)
//     Write(data) -> lay written = Inner.Write(data), dao nguoc chuoi written (vd dung .Reverse().ToArray()),
//       tra ve "[encrypted]{chuoi da dao nguoc}"
