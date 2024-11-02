using StoreAgent.Models;

namespace StoreAgent.Services;

public interface IProductService {

    public void AddProduct(Product prod);

    public List<ProductSearchResult> GetSimilarProducts(float[] descriptionEmbedding, string department);

    public string[] GetDepartmentNames(); 
}