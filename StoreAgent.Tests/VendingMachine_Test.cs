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
        public void Test_GetDepartmentNames_ReturnsCorrectDepartments()
        {
            // Arrange
            var expectedDepartments = new string[] { "Department1", "Department2" };

            // Act
            var result = _vendingMachine.ProductService.GetDepartmentNames();

            // Assert
            Assert.Equal(expectedDepartments, result);
        }

        [Fact]
        public void Test_Engage() {
            _vendingMachine.Engage();
            Assert.NotNull(_vendingMachine.MessageForCustomer);
        }

        
    }
}
