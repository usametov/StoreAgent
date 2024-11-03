using System.Linq;
using StoreAgent.Models;
using System.Diagnostics;
using StoreAgent.Helpers;

namespace StoreAgent.Services;

public class ProductService : IProductService
{
    private List<Product> products;     

    public ProductService() {
        this.products = new List<Product>();
    }

    public List<Product> GetAllProducts() => this.products;     

    public void AddProduct(Product prod)
    {
        this.products.Add(prod);
    }

    public void AddAllProducts(List<Product> prods) 
    {
        this.products.AddRange(prods);
    }

    public string[] GetDepartmentNames()
    {
        return products.Select(p=>p.Department).Distinct().ToArray();
    }

    public List<ProductSearchResult> GetSimilarProducts(float[] descriptionEmbedding, 
            string department, int topK, decimal minPrice, decimal maxPrice, double threshold)
    {
        // Ensure products have good embeddings
        Debug.Assert(products.All(p => p.Embedding != null && p.Embedding.Length > 0));
                
        // Filter products by department
        var filteredProducts = products.Where(p => p.Department == department)
                                       .Where(p => minPrice < p.Price && p.Price < maxPrice)
                                       .ToList();

        // Calculate cosine similarity for each product
        var similarProducts = filteredProducts
            .Select(p => new
            {
                Product = p,
                Similarity = CommonUtils.CalculateCosineSimilarity(descriptionEmbedding, p?.Embedding)
            })
            .Distinct()
            .Where(p=>p.Similarity >= threshold)
            .OrderByDescending(p => p.Similarity)
            .Select(p => new ProductSearchResult{Product = p.Product, Score = p.Similarity})            
            .OrderByDescending(p=>p.Score)
            .Take(topK)
            .ToList();

        return similarProducts;
    }

    
}
