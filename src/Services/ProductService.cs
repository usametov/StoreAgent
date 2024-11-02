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


    public void AddProduct(Product prod)
    {
        this.products.Add(prod);
    }

    public string[] GetDepartmentNames()
    {
        return products.Select(p=>p.Department).Distinct().ToArray();
    }

    public List<ProductSearchResult> GetSimilarProducts(float[] descriptionEmbedding, string department)
    {
        // Ensure products have embeddings
        Debug.Assert(products.All(p => p.Embedding != null));
                
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
            .Select(p => new ProductSearchResult{Product = p.Product, Score = p.Similarity})
            .ToList();

        return similarProducts;
    }

    
}
