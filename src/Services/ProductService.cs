using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
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

    public List<Product> DeserializeProductsFromJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file {filePath} does not exist.");
        }

        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Product>>(jsonString);
    }
}
