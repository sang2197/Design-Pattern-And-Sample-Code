# Mediator Pattern (MediatR)

## 1. Khái niệm

**Mediator** là một behavioral design pattern, dùng để giảm phụ thuộc giữa nhiều đối tượng bằng cách bắt chúng giao tiếp qua một đối tượng trung gian (mediator), thay vì gọi thẳng lẫn nhau:

- **Không có Mediator:** nơi gọi (Controller/Service) phải biết và gọi trực tiếp đến từng class xử lý cụ thể → số lượng phụ thuộc tăng nhanh khi hệ thống lớn dần.
- **Có Mediator:** nơi gọi chỉ cần biết duy nhất 1 interface (`IMediator`), gửi một "yêu cầu" (Request) vào đó; Mediator tự tìm đúng Handler tương ứng để xử lý, nơi gọi không cần biết Handler nào tồn tại.

**MediatR** là thư viện .NET hiện thực hoá pattern này, thường đi kèm CQRS: Request chính là Command hoặc Query, Handler là nơi xử lý tương ứng.

## 2. Ý nghĩa

- Cắt đứt phụ thuộc trực tiếp giữa nơi gọi và nơi xử lý — Controller chỉ phụ thuộc `IMediator`, không phụ thuộc bất kỳ Handler cụ thể nào.
- Tuân thủ Open/Closed Principle: thêm nghiệp vụ mới chỉ cần thêm 1 class Request + Handler mới, không phải sửa code Controller cũ.
- Tách nhỏ logic theo từng use-case, mỗi Handler chỉ xử lý đúng 1 việc, thay vì dồn hết vào 1 Service khổng lồ.

## 3. Code mẫu

### Ví dụ 1 — Định nghĩa Request và Handler

    public class DeleteProductCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteProductCommand(int id)
        {
            Id = id;
        }

        public class Handler : IRequestHandler<DeleteProductCommand, Unit>
        {
            private readonly WriteDataContext _dataContext;

            public Handler(WriteDataContext dataContext)
            {
                _dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
            {
                var entity = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null)
                {
                    throw new ArgumentException("Product not found");
                }

                _dataContext.Products.Remove(entity);
                await _dataContext.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

### Ví dụ 2 — Gọi trên Controller/Router

    [ApiController, Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return NoContent();
        }
    }

### Ví dụ 3 — Gọi từ Handler khác (xóa A kéo theo xóa B)

    public class DeleteProductCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteProductCommand(int id)
        {
            Id = id;
        }

        public class Handler : IRequestHandler<DeleteProductCommand, Unit>
        {
            private readonly WriteDataContext _dataContext;
            private readonly IMediator _mediator;

            public Handler(WriteDataContext dataContext, IMediator mediator)
            {
                _dataContext = dataContext;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
            {
                var entity = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null)
                {
                    throw new ArgumentException("Product not found");
                }

                // Handler A gọi sang Handler B qua Mediator, không cần biết Handler B xử lý thế nào
                await _mediator.Send(new DeleteProductImagesByProductIdCommand(entity.Id), cancellationToken);

                _dataContext.Products.Remove(entity);
                await _dataContext.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class DeleteProductImagesByProductIdCommand : IRequest<Unit>
    {
        public int ProductId { get; set; }

        public DeleteProductImagesByProductIdCommand(int productId)
        {
            ProductId = productId;
        }

        public class Handler : IRequestHandler<DeleteProductImagesByProductIdCommand, Unit>
        {
            private readonly WriteDataContext _dataContext;

            public Handler(WriteDataContext dataContext)
            {
                _dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteProductImagesByProductIdCommand request, CancellationToken cancellationToken)
            {
                var images = _dataContext.ProductImages.Where(x => x.ProductId == request.ProductId);
                _dataContext.ProductImages.RemoveRange(images);
                await _dataContext.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
