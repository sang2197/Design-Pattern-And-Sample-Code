namespace Exercises.Proxy.Example3;

// Proxy - Vi du 3: Caching Proxy - cache lai ket qua truy van ton kem
// Xem lai: Proxy-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IProductRepository (Subject) { string GetProductById(int id); }
// - class ProductRepository : IProductRepository (RealSubject)
//     int QueryCount { get; private set; }
//     GetProductById(id) -> tang QueryCount, tra ve "Product-{id}"
// - class CachingProductRepositoryProxy : IProductRepository (Proxy)
//     private readonly ProductRepository _realRepository;
//     private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();
//     constructor nhan ProductRepository realRepository
//     GetProductById(id) -> neu _cache co san id thi tra ve gia tri cache;
//       nguoc lai goi _realRepository.GetProductById(id), luu vao _cache roi tra ve
