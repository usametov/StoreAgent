using StoreAgent.Helpers;
using Xunit;

namespace StoreAgent.Tests
{
    public class CommonUtilsTests
    {
        [Fact]
        public void DeserializeProductsFromJsonFile_ValidJson_ReturnsListOfProducts()
        {
            // Arrange
            var json = @"[
                {
                    ""SKU"": ""ZYXWVUTS"",
                    ""name"": ""Smartphone Case"",
                    ""department"": ""electronics"",
                    ""description"": ""Protective case for your smartphone."",
                    ""price"": 14.99
                },
                {
                    ""SKU"": ""GFEDCBAH"",
                    ""name"": ""Organic Apples"",
                    ""department"": ""groceries"",
                    ""description"": ""Fresh organic apples from local farms."",
                    ""price"": 6.49
                }
            ]";
            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, json);

            // Act
            var products = CommonUtils.DeserializeProductsFromJsonFile(filePath);

            // Assert
            Assert.Equal(2, products.Count);
            Assert.Equal("ZYXWVUTS", products[0].SKU);
            Assert.Equal("Smartphone Case", products[0].Name);
            Assert.Equal("electronics", products[0].Department);
            Assert.Equal("Protective case for your smartphone.", products[0].Description);
            Assert.Equal(14.99m, products[0].Price);

            Assert.Equal("GFEDCBAH", products[1].SKU);
            Assert.Equal("Organic Apples", products[1].Name);
            Assert.Equal("groceries", products[1].Department);
            Assert.Equal("Fresh organic apples from local farms.", products[1].Description);
            Assert.Equal(6.49m, products[1].Price);

            // Clean up
            File.Delete(filePath);
        }
    }
}
