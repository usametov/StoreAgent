using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;

namespace StoreAgent;
public class Dispatcher {
    private IAIService aiService;
    private IProductService productService;

    public Dispatcher() {        
    }

    public void Init() {
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new string[]{});
        builder.Services.AddSingleton<IAIService, OpenAIService>();
        builder.Services.AddSingleton<IProductService, ProductService>();
        using IHost host = builder.Build();
        host.RunAsync();
        Debug.Print("host is running");

        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        this.aiService = provider.GetRequiredService<IAIService>();
        this.productService = provider.GetRequiredService<IProductService>();
        
        this.aiService.ChatEndpoint = ConfigurationManager.GetAzureOpenAIEndpoint();
        this.aiService.EmbeddingEndpoint = ConfigurationManager.GetAzureOpenAIEmbeddingEndpoint();
        this.aiService.Key = ConfigurationManager.GetAzureOpenAIApiKey();
        this.aiService.Init();

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information("done Init");
    }

    public string TestOpenAI(string txt) {
        Debug.Assert(this.aiService!=null);
        
        var result= this.aiService.GenerateEmbedding(txt);
        //Log.Information(String.Join(",", result.Take(5)));
        return result.GetValue(2).ToString();
    }

    public void LoadProducts() 
    {
        var productDB = "/workspaces/StoreAgent/src/Repositories/Products.json";
        var products = CommonUtils.DeserializeProductsFromJsonFile(productDB);    
        Log.Information($"loaded {products.Count} products");
        Log.Information($"Product 1: {products[0].Name}, {products[0].Description}, {products[0].Price}, {products[0].SKU}");

        var inflatedProducts = CommonUtils.InflateProductEmbeddings(products, this.aiService);        
        foreach(var prod in inflatedProducts) 
        {
            this.productService.AddProduct(prod);
        }

        Log.Information($"inflated {this.productService.GetProducts("", "").Count} products");
        Log.Information(String.Join(",", inflatedProducts[0].Embedding.Take(10)));
        Log.Information($"Department names: {String.Join(",", this.productService.GetDepartmentNames())}");
    }
}