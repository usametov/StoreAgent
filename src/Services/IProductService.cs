using StoreAgent.Models;

namespace StoreAgent;

public interface IProductService {

    public void AddProduct(Product prod);

    public List<Product> GetProducts(string description, string department);

    public string[] GetDepartmentNames(); 
}