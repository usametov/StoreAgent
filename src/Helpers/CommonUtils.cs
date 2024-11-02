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

    public static AIResponse DeserializeAIResponse(string response) 
    {
        Debug.Assert(response!=null);
        return JsonSerializer.Deserialize<AIResponse>(response); 
    }

    public static double CalculateCosineSimilarity(float[] array1, float[] array2)
    {
        Debug.Assert(array1.Length == array2.Length, "Arrays must be of the same size.");

        double dotProduct = 0.0;
        double magnitude1 = 0.0;
        double magnitude2 = 0.0;

        for (int i = 0; i < array1.Length; i++)
        {
            dotProduct += array1[i] * array2[i];
            magnitude1 += array1[i] * array1[i];
            magnitude2 += array2[i] * array2[i];
        }

        magnitude1 = Math.Sqrt(magnitude1);
        magnitude2 = Math.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0.0;
        }

        return dotProduct / (magnitude1 * magnitude2);
    }
}
