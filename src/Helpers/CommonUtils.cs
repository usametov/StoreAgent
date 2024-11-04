using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using StoreAgent.Models;
using StoreAgent.Services;
using OpenAI.Chat;
using Serilog;
using Stateless.Graph;
using Stateless;

namespace StoreAgent.Helpers;
public class CommonUtils {

    public static List<Product> DeserializeProductsFromJsonFile(string filePath)
    {
        Debug.Assert(File.Exists(filePath));

        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Product>>(jsonString) ?? new List<Product>(); 
    }
    public static List<Product> InflateProductEmbeddings(
                            List<Product> products, 
                            IAIService aiService) 
    {
        foreach(var prod in products) {
            var fullDescription = $"Name: {prod.Name}, Description: {prod.Description}";
            prod.Embedding = aiService.GenerateEmbedding(fullDescription);        
        }

        return products;
    }

    public static AIResponse DeserializeAIResponse(string response) 
    {
        Debug.Assert(response!=null);
        if(Log.Logger == null)
            Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                                  .CreateLogger();    
                
        var cleanJson = response.Replace("```json", "").Replace("```", "");
        //Log.Information(cleanJson);
        return JsonSerializer.Deserialize<AIResponse>(cleanJson); 
    }

    public static double CalculateCosineSimilarity(float[] array1, float[] array2)
    {
        Debug.Assert(array1 != null && array2 != null, "Arrays must not be null.");
        Debug.Assert(array1.Length == array2.Length, "Arrays must be of the same size.");
        Debug.Assert(array1.Length > 0, "Arrays must not be empty.");

        var dotProduct = array1.Zip(array2, (a, b) => a * b).Sum();
        var magnitude1 = Math.Sqrt(array1.Select(a => a * a).Sum());
        var magnitude2 = Math.Sqrt(array2.Select(a => a * a).Sum());

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        return dotProduct / (magnitude1 * magnitude2);
    }

    public static string StringifyAIResponse(ChatCompletion content) 
    {
        return string.Join(System.Environment.NewLine, content.Content.Select(c=>c.Text));
    }

    public static string StringifyProductSearchResult(List<ProductSearchResult> searchResult) 
    {
        if(searchResult == null || searchResult.Count == 0)
            return string.Empty;
            
        var rows = searchResult.Select(p => $"{p.Product.SKU} - {p.Product.Name}, {p.Product.Description}, {p.Product.Price}, Score: {p.Score}");
        return string.Join(System.Environment.NewLine, rows);
    }

    public static bool IsValid(ConversationIntent intent) {

        return !string.IsNullOrEmpty(intent?.ProductDescription);
    }

    public static void ExportWorkflowToDotGraph(VendingMachine vendingMachine)
    {
        string dotGraph = UmlDotGraph.Format(vendingMachine.GetInfo());
        File.WriteAllText("../vending-machine.dot", dotGraph);
    }

    public static List<OrderItem> TryParseSKUs(string inquiry, List<ProductSearchResult> searchResult) 
    {
        var validSKUs = searchResult.Select(p=>p.Product.SKU).ToHashSet();
        var skuQtyPairs = inquiry.Split(",");

        try {
        return skuQtyPairs.Select(pair => 
                            {
                                var skuQtyPair = pair.Split(":"); 
                                var product = new Product{SKU = skuQtyPair[0].Trim(), 
                                                        Name = "", Description = "", Department = ""};
                                return new OrderItem{Product=product, Quantity = uint.Parse(skuQtyPair[1].Trim())};
                            })
                            .Join(searchResult,
                                  o => o.Product.SKU,
                                  s => s.Product.SKU,
                                  (o, s) => new OrderItem { Product = s.Product, Quantity = o.Quantity })
                            .Where(oi=>validSKUs.Contains(oi.Product.SKU))
                            .ToList();
        }
        catch(FormatException) {
            return new List<OrderItem>();
        }
    }

    
}
