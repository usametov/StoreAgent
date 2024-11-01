using System.Diagnostics;
using System.Text.Json;
using StoreAgent.Models;
using StoreAgent.Services;

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
}