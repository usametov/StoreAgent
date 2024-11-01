using System.Text.Json.Serialization;

namespace StoreAgent.Models;

public class Product
{
    [JsonPropertyName("SKU")]
    public required string SKU { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("department")]
    public required string Department { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonIgnore]
    public float[]? Embedding {get; set;}
}
