# Introduction 
A simple CQRS aggregator library. 

# Getting Started
This library helps to follow CQRS pattern by segregating commands and queries via IQueryHandler and ICommandHandler. Further the commands and queries can be send by IUtiQ contract.

# How To Use

Let's create a repository which get and create a product in Product table.

    public interface IProductRepository
    {
        Task<IEnumerable<Product>> Get(Expression<Func<Product, bool>> expression);
        
        Task Create(Product product);
    }

And a product class

    public class Product
    {
        public Guid Id { get; }
        public string Sku { get; }
        public string Name { get; }
        public string? Description { get; }
    }

## Queries
Create a model class/Query. In our example let's assume ```ProductQuery.cs``` is our query class.

    public class ProductQuery
    {
        public Guid Id { get; }

        public ProductQuery(Guid id)
        {
            Id = id;
        }
    }

Now create a handler class as ```ProductQueryHandler.cs``` class and implement ```IQueryHandler<T, TResult>```.

    public class ProductQueryHandler : IQueryHandler<ProductQuery>
    {
        private readonly IProductRepository _productRepository;

        public ProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(ProductQuery query, CancellationToken cancellationToken)
        {
            return await _productRepository.Get(x => x.Id == query.Id);
        }
    }

Access this query by initializing ```IUtiQ```

    private readonly IUtiQ _utiq

and send query as

    await _utiq.SendQuery<ProductQuery, Product>(new ProductQuery(id));

## Command
Create a command. Our command class is ```ProductCommand.cs```

    public class ProductCommand
    {
        public Guid Id { get; }
        public string Sku { get; }
        public string Name { get; }
        public string? Description { get; }

        public ProductCommand(Guid id, string sku, string name, string? description)
        {
            Id = id;
            Sku = sku;
            Name = name;
            Description = description;
        }
    }

Now create a handler class as ```ProductCommandHandler.cs``` class and implement ```ICommandHandler<T>```.

    public class ProductCommandHandler : ICommandHandler<ProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public ProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(ProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product()
            {
                Id = command.Id,
                Sku = command.Sku,
                Name = command.Name,
                Description = command.Description
            };

            await _productRepository.Create(product);
        }
    }

### Dependencies

Add dependencies

    services.AddCommandHandler<ProductCommand, ProductCommandHandler>();
    services.AddQueryHandler<ProductQuery, Product, ProductQueryHandler>()

Access this query by initializing ```IUtiQ```

    private readonly IUtiQ _utiq

send query as

    await _utiq.SendQuery<ProductQuery, Product>(new ProductQuery(id));

and send command as

    await _utiq.SendCommand(new ProductCommand(productDto));