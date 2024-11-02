using System.Linq;
using StoreAgent.Models;

namespace StoreAgent.Services;

public class ProductService : IProductService
{
    private List<Product> products; 
    public ProductService() {
        this.products = new List<Product>();
    }
    public void AddProduct(Product prod)
    {
        this.products.Add(prod);
    }

    public string[] GetDepartmentNames()
    {
        return products.Select(p=>p.Department).Distinct().ToArray();
    }

    public List<Product> GetSimilarProducts(string description, string department)
    {
        // Ensure products have embeddings
        if (products.Any(p => p.Embedding == null))
        {
            throw new InvalidOperationException("Some products do not have embeddings.");
        }

        // Generate embedding for the description
        var descriptionEmbedding = aiService.GenerateEmbedding(description);

        // Filter products by department
        var departmentProducts = products.Where(p => p.Department == department).ToList();

        // Calculate cosine similarity for each product
        var similarProducts = departmentProducts
            .Select(p => new
            {
                Product = p,
                Similarity = CommonUtils.CalculateCosineSimilarity(descriptionEmbedding, p.Embedding)
            })
            .OrderByDescending(p => p.Similarity)
            .Select(p => p.Product)
            .ToList();

        return similarProducts;
    }

    
}
