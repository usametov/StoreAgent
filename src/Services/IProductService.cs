using StoreAgent.Models;

namespace StoreAgent.Services;

public interface IProductService {

    public void AddProduct(Product prod);

    public void AddAllProducts(List<Product> prods);

    public List<Product> GetAllProducts();

    public List<ProductSearchResult> GetSimilarProducts(float[] descriptionEmbedding, 
                string department, int topK, decimal minPrice, decimal maxPrice, 
                double threshold);

    public string[] GetDepartmentNames(); 
}