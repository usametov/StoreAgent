using System.Linq;
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

    public string[] GetDepartmentNames()
    {
        return products.Select(p=>p.Department).Distinct().ToArray();
    }

    public List<Product> GetProducts(string description, string department)
    {
        //TODO: add search
        return products;
    }

    
}
