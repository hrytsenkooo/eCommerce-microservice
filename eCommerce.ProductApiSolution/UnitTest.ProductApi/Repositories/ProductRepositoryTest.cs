using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb").Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
        {
            // Arrange 
            var existingProduct = new Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already added");
        }

        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnsSuccessResponse()
        {
            // Arrange
            var product = new Product() { Name = "New Product" };

            // Act
            var result = await productRepository.CreateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product added to database successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnsSuccessResponse()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 78.67m, Quantity = 5 };
            productDbContext.Products.Add(product);

            // Act
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Existing Product is deleted successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var product = new Product() { Id = 2, Name = "NonExistingProduct", Price = 78.67m, Quantity = 5 };

            // Act
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("NonExistingProduct not found");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnProduct()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "ExistingProduct", Price = 78.67m, Quantity = 5 };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.FindByIdAsync(product.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("ExistingProduct");
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Product 1" },
                new() { Id = 2, Name = "Product 2" }
            };

            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            // Arrange
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 2";

            // Act
            var result = await productRepository.GetByAsync(predicate);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProduct_WhenProductIsUpdatedSuccessfully_ReturnSuccessResponse()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1" };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.UpdateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 is updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsNotFound_ReturnErrorResponse()
        {
            // Arrange
            var updatedProduct = new Product { Id = 1, Name = "Product 1" };

            // Act
            var result = await productRepository.UpdateAsync(updatedProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 1 not found");
        }
    }
}
