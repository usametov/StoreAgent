using StoreAgent.Helpers;
using StoreAgent.Models;
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

        [Fact]
        public void DeserializeAIResponseFromJsonFile1() 
        {
            var json = @"{
                    ""FreeText"": ""Absolutely! We have several packs of diapers available under $30. Let me check the options for you."",
                    ""ConversationIntent"": {
                    ""ProductDescription"": ""diapers"",
                    ""Department"": ""kids"",
                    ""Quantity"": 2,
                    ""minPrice"": 0.01,
                    ""maxPrice"": 30
                    }
                }";

            var response = CommonUtils.DeserializeAIResponse(json);                    
            Assert.Equal("Absolutely! We have several packs of diapers available under $30. Let me check the options for you.", response.FreeText);
            Assert.Equal("diapers", response.ConversationIntent.ProductDescription);
            Assert.Equal("kids", response.ConversationIntent.Department);
            Assert.Equal(2, response.ConversationIntent.Quantity);
            Assert.Equal(0.01m, response.ConversationIntent.minPrice);
            Assert.Equal(30m, response.ConversationIntent.maxPrice);
        }
    
        [Fact]
        public void TryParseSKUs_ValidInput_ReturnsParsedOrderItems()
        {
            // Arrange
            var inquiry = "SKU1:2 ,SKU2:3";
            var searchResult = new List<ProductSearchResult>
            {
                new ProductSearchResult { 
                    Product = new Product { SKU = "SKU1",
                                            Department = "",
                                            Name = "",
                                            Description = "" }, Score = 0.9 },
                new ProductSearchResult { 
                    Product = new Product { SKU = "SKU2",
                                            Department = "",
                                            Name = "",
                                            Description = "" }, Score = 0.9 }
            };

            // Act
            var result = CommonUtils.TryParseSKUs(inquiry, searchResult);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("SKU1", result[0].Product.SKU);
            Assert.Equal(2u, result[0].Quantity);
            Assert.Equal("SKU2", result[1].Product.SKU);
            Assert.Equal(3u, result[1].Quantity);
        }

        [Fact]
        public void TryParseSKUs_InvalidInput_ReturnsNull()
        {
            // Arrange
            var inquiry = "SKU1:invalid,SKU2:3";
            var searchResult = new List<ProductSearchResult>
            {
                new ProductSearchResult { 
                    Product = new Product { SKU = "SKU1",
                                            Department = "",
                                            Name = "",
                                            Description = ""}, Score =0.9 },
                new ProductSearchResult { 
                    Product = new Product { SKU = "SKU2",
                                            Department = "",
                                            Name = "",
                                            Description = "" }, Score =0.9 }
            };

            // Act
            var result = CommonUtils.TryParseSKUs(inquiry, searchResult);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ExportWorkflowToDotGraph_CreatesDotGraphFile()
        {
            // Arrange
            var vendingMachine = new StateMachine<VendingMachine, ConversationTrigger>(() => new VendingMachine(), v => { });
            vendingMachine.Configure(VendingMachine.Off)
                          .Permit(ConversationTrigger.StartConversation, VendingMachine.On);

            var expectedDotGraph = "digraph G {\n  Off -> On [label=\"StartConversation\"];\n}";

            // Act
            CommonUtils.ExportWorkflowToDotGraph(vendingMachine);

            // Assert
            Assert.True(File.Exists("vending-machine.dot"));
            var actualDotGraph = File.ReadAllText("vending-machine.dot");
            Assert.Equal(expectedDotGraph, actualDotGraph);

            // Clean up
            File.Delete("vending-machine.dot");
        }
    }
}
