using System.Text.Json.Serialization;

namespace StoreAgent.Models;

public class Product
{
    [JsonPropertyName("SKU")]
    public string SKU { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("department")]
    public string Department { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonIgnore]
    public float[] Embedding {get; set;}
}
