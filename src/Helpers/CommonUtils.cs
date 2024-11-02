using System.Diagnostics;
using System.Linq;
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

    public static AIResponse DeserializeAIResponse(string response) 
    {
        Debug.Assert(response!=null);
        return JsonSerializer.Deserialize<AIResponse>(response); 
    }

    public static double CalculateCosineSimilarity(float[] array1, float[] array2)
    {
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
}
