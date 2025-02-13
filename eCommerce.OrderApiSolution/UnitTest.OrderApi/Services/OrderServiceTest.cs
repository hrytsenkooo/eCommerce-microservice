using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        private readonly IOrder orderInterface;
        public OrderServiceTest() 
        {
            orderInterface = A.Fake<IOrder>();
            orderServiceInterface = A.Fake<IOrderService>();
        }

        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("https://localhost")
            };
            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            // Arrange
            int productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 56.78m);
            var _httpClient = CreateFakeHttpClient(productDTO);

            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            // Arrange
            int productId = 1;
            var _httpClient = CreateFakeHttpClient(null!);
            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersByClientIs_OrderExist_ReturnOrderDetails()
        {
            // Arrange
            int clientId = 1;
            var orders = new List<Order>
            {
                new() { Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.Now },
                new() { Id = 1, ProductId = 2, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.Now }
            };

            A.CallTo(() => orderInterface.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var _orderService = new OrderService(orderInterface, null!, null!);

            // Act
            var result = await _orderService.GetOrdersByClientId(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(orders.Count);
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}