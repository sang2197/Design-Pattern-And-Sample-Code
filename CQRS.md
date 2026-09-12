# CQRS (Command Query Responsibility Segregation)

## 1. Khái niệm

**CQRS** (Command Query Responsibility Segregation) là một design pattern tách biệt Lệnh thực thi và Truy vấn, thay vì dùng chung một model/service để vừa đọc vừa ghi dữ liệu, hệ thống được tách thành hai luồng xử lý độc lập:

- **Command side (Write model):** chịu trách nhiệm xử lý các yêu cầu làm thay đổi dữ liệu (tạo, sửa, xóa). Command không trả về dữ liệu nghiệp vụ.
- **Query side (Read model):** chịu trách nhiệm xử lý các yêu cầu đọc dữ liệu, trả về dữ liệu cho client. Query không được phép làm thay dữ liệu.

## 2. Ý nghĩa

- CQRS thừa nhận một thực tế: **đọc và ghi có đặc tính, khối lượng, yêu cầu tối ưu hoàn toàn khác nhau**. Đa số hệ thống có tỷ lệ đọc cao hơn ghi rất nhiều lần, nhưng logic nghiệp vụ khi ghi (validate, tính toán, ràng buộc) lại phức tạp hơn nhiều so với đọc.
- Nếu dùng chung một model cho cả hai, model đó buộc phải thỏa hiệp: đủ chặt chẽ để đảm bảo tính đúng đắn khi ghi, nhưng cũng đủ linh hoạt để phục vụ mọi kiểu truy vấn hiển thị — dẫn đến model ngày càng phình to, khó bảo trì, và không tối ưu cho cả hai phía.
- CQRS giải quyết mâu thuẫn đó bằng cách **để mỗi phía tối ưu theo đúng mục đích của nó**, thay vì tìm một điểm cân bằng chung.


## 3. Code mẫu

Ba ví dụ dưới đây minh họa CQRS ở các

### Ví dụ 1 — Command thêm mới dữ liệu

public class CreateProductCommand : IRequest<Unit>
{
    public CreateProductModel Model { get; set; }

    public CreateProductCommand(CreateProductModel model)
    {
        Model = model;
    }

    public class Handler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly WriteDataContext _dataContext;

        public Handler(WriteDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            var entity = AutoMapperUtils.AutoMap<CreateProductModel, Product>(model);

            await _dataContext.Products.AddAsync(entity);
            await _dataContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

### Ví dụ 2 — Query lấy thông tin bản ghi

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

        public Handler(ReadDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ProductModel> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var entity = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return AutoMapperUtils.AutoMap<Product, ProductModel>(entity);
        }
    }
}


### Ví dụ 3 - Cấu hình 2 database

#region Config database
string systemDBWrite = String.Format(configuration["Database:System:ConnectionString:MSSQLDatabase"], configuration["uid"], configuration["password"]);
services.AddDbContext<WriteDataContext>(x =>
{
    x.UseSqlServer(systemDBWrite);
    x.EnableSensitiveDataLogging();
});

string systemDBRead = String.Format(configuration["Database:System:ConnectionString:MSSQLDatabaseRead"], configuration["uid"], configuration["password"]);
services.AddDbContext<ReadDataContext>(x =>
{
    x.UseSqlServer(systemDBRead);
    x.EnableSensitiveDataLogging();
});
#endregion Config database