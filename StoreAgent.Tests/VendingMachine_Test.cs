using Moq;
using StoreAgent;
using StoreAgent.Services;
using Xunit;

namespace StoreAgent.Tests
{
    public class VendingMachine_Test
    {
        private readonly Mock<IAIService> _mockAIService;
        private readonly Mock<IProductService> _mockProductService;
        private readonly VendingMachine _vendingMachine;

        public VendingMachine_Test()
        {
            _mockAIService = new Mock<IAIService>();
            _mockProductService = new Mock<IProductService>();
            _vendingMachine = new VendingMachine
            {
                ProductService = _mockProductService.Object
            };
        }

        [Fact]
        public void Test_GetInfo_ReturnsNotNull()
        {
            // Arrange
            // Act
            var result = _vendingMachine.GetInfo();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_ProductService_SetAndGet()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();

            // Act
            _vendingMachine.ProductService = mockProductService.Object;

            // Assert
            Assert.Equal(mockProductService.Object, _vendingMachine.ProductService);
        }
    }
}
