# Cache-Aside Pattern

## 1. Khái niệm

**Cache-Aside** (còn gọi là Lazy Loading Cache) là một caching pattern trong đó Application chịu trách nhiệm quản lý việc đọc/ghi giữa cache và database chính:

- **Khi đọc:** Application kiểm tra cache trước → nếu có dữ liệu (cache hit) thì trả về ngay, không đụng tới DB; nếu không có (cache miss) → đọc từ DB → lưu kết quả vào cache → trả về.
- **Khi ghi/cập nhật/xóa:** ứng dụng ghi thẳng vào DB, sau đó xóa hoặc cập nhật lại entry tương ứng trong cache để tránh lấy nhầm dữ liệu cũ.

Cache không tự động đồng bộ với DB (khác với Read-Through/Write-Through, nơi cache tự lo việc đó) — toàn bộ trách nhiệm nằm ở code.

## 2. Ý nghĩa

- Giảm tải cho DB với dữ liệu đọc nhiều, ít thay đổi (danh mục, cấu hình, combobox...) bằng cách phục vụ trực tiếp từ bộ nhớ (Redis, in-memory cache) thay vì query lại DB mỗi lần.
- Cache và DB là hai hệ thống độc lập — cache có thể mất (crash, restart, hết hạn) mà không ảnh hưởng tính đúng đắn của dữ liệu, vì DB luôn là nguồn chân lý (source of truth), cache chỉ là bản sao tăng tốc.

**Đánh đổi cần lưu ý:** nếu quên reset cache sau khi ghi dữ liệu, hệ thống sẽ trả về dữ liệu cũ cho tới khi cache tự hết hạn.

## 3. Code mẫu

### Ví dụ 1 — Đọc dữ liệu qua Cache-Aside (miss thì query DB rồi lưu lại cache)

public class GetProductByIdQuery : IRequest<ProductModel>
{
    public int Id { get; set; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }

    public class Handler : IRequestHandler<GetProductByIdQuery, ProductModel>
    {
        private readonly ReadDataContext _dataContext;
        private readonly ICacheService _cacheService;

        public Handler(ReadDataContext dataContext, ICacheService cacheService)
        {
            _dataContext = dataContext;
            _cacheService = cacheService;
        }

        public async Task<ProductModel> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = ProductCacheKey.Build(request.Id);

            return await _cacheService.GetOrCreate(cacheKey, async () =>
            {
                var entity = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);
                return AutoMapperUtils.AutoMap<Product, ProductModel>(entity);
            });
        }
    }
}

### Ví dụ 2 — Reset cache khi dữ liệu bị thay đổi

public class UpdateProductCommand : IRequest<Unit>
{
    public UpdateProductModel Model { get; set; }

    public UpdateProductCommand(UpdateProductModel model)
    {
        Model = model;
    }

    public class Handler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly WriteDataContext _dataContext;
        private readonly ICacheService _cacheService;

        public Handler(WriteDataContext dataContext, ICacheService cacheService)
        {
            _dataContext = dataContext;
            _cacheService = cacheService;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.Model.Id);
            if (entity == null)
            {
                throw new ArgumentException("Product not found");
            }

            entity.Name = request.Model.Name;
            entity.Price = request.Model.Price;

            _dataContext.Products.Update(entity);
            await _dataContext.SaveChangesAsync();

            // Dữ liệu đã đổi -> xóa cache cũ, lần đọc kế tiếp sẽ nạp lại dữ liệu mới
            _cacheService.Remove(ProductCacheKey.Build(entity.Id));

            return Unit.Value;
        }
    }
}

### Ví dụ 3 — Cache Key Builder (tổ chức key theo prefix, tránh đụng key giữa các loại dữ liệu)

public static class ProductCacheKey
{
    private const string Prefix = "PRODUCT";

    public static string Build(int id)
    {
        return $"{Prefix}-{id}";
    }

    public static string BuildListKey()
    {
        return $"{Prefix}-LIST";
    }
}
