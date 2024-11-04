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
            _mockProductService.Setup(service => service.GetDepartmentNames())
                               .Returns(new string[] { "Department1", "Department2" });
            _mockAIService.Setup(service => service.GenerateEmbedding(It.IsAny<string>()))
                          .Returns(new float[] { 0.1f, 0.2f, 0.3f });
            _mockProductService.Setup(service => service.GetSimilarProducts(
                It.IsAny<float[]>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(new List<ProductSearchResult>
                {
                    new ProductSearchResult { Product = new Product { Name = "Product1" }, SimilarityScore = 0.9f },
                    new ProductSearchResult { Product = new Product { Name = "Product2" }, SimilarityScore = 0.8f }
                });
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
        public void Test_Engage()
        {
            _vendingMachine.Engage();
            Assert.NotNull(_vendingMachine.MessageForCustomer);            
        }

        [Fact]
        public void Test_search()
        {
            // prepare vendingmachine
            _vendingMachine.QueryEmbedding = _mockAIService.Object.GenerateEmbedding("test");

            // Assert
            //Assert.Equal(expectedEmbedding, result);
        }


    }
}
