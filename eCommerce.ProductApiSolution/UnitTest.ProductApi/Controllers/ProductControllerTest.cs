using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductControllerTest()
        {
            productInterface = A.Fake<IProduct>();
            productsController = new ProductsController(productInterface);
        }

        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProducts()
        {
            // Arrange
            var products = new List<Product>()
            {
                new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m },
                new() { Id = 2, Name = "Product 2", Quantity = 110, Price = 1004.70m }
            };

            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            // Arrange
            var products = new List<Product>();

            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected in the database");
        }

        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            productsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSuccessfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Created");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Created");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "Failed");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult!.Message.Should().Be("Failed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(true, "Update");

            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Update");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "Update Failed");

            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult!.Message.Should().Be("Update Failed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccessful_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(true, "Deleted successfully");

            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Deleted successfully");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "Delete failed");

            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult!.Message.Should().Be("Delete failed");
            responseResult!.Flag.Should().BeFalse();
        }
    }
}
