using StoreAgent.Models;

namespace StoreAgent;

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

    public List<Product> GetProducts(string description, string department)
    {
        throw new NotImplementedException();
    }
}