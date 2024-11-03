using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;
using StoreAgent.Helpers;
using StoreAgent.Services;
using StoreAgent.Models;

namespace StoreAgent;

public class Dispatcher {
    private IAIService? aiService;
    private IProductService? productService;
    private VendingMachine workflow = new VendingMachine();  

    private IShoppingCart? shoppingCart;

    public Dispatcher() {              
    }

    public void Init() {
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new string[]{});
        builder.Services.AddSingleton<IAIService, OpenAIService>();
        builder.Services.AddSingleton<IProductService, ProductService>();
        builder.Services.AddSingleton<IShoppingCart, ShoppingCart>();
        using IHost host = builder.Build();
        host.RunAsync();
        Debug.Print("host is running");

        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        this.aiService = provider.GetRequiredService<IAIService>();
        this.productService = provider.GetRequiredService<IProductService>();
        this.shoppingCart = provider.GetRequiredService<IShoppingCart>();
        
        this.aiService.ChatEndpoint = ConfigurationManager.GetAzureOpenAIEndpoint();
        this.aiService.EmbeddingEndpoint = ConfigurationManager.GetAzureOpenAIEmbeddingEndpoint();
        this.aiService.Key = ConfigurationManager.GetAzureOpenAIApiKey();
        this.aiService.Init();

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information("done Init");
    }

    public string? TestOpenAI(string txt) {
        Debug.Assert(this.aiService!=null);
        
        var result= this.aiService.GenerateEmbedding(txt);        
        return result?.GetValue(2)?.ToString();
    }

    public void LoadProducts() 
    {
        Debug.Assert(this.aiService!=null);
        var productDB = "/workspaces/StoreAgent/src/Repositories/Products.json";
        var products = CommonUtils.DeserializeProductsFromJsonFile(productDB);    
        Log.Information($"loaded {products.Count} products");
        Log.Information($"Product 1: {products[0].Name}, {products[0].Description}, {products[0].Price}, {products[0].SKU}");

        var inflatedProducts = CommonUtils.InflateProductEmbeddings(products, this.aiService);        
        foreach(var prod in inflatedProducts) 
        {
            this.productService?.AddProduct(prod);
        }

        //Log.Information($"inflated {this.productService?.GetSimilarProducts("", "")?.Count} products");        
        Log.Information($"Department names: {String.Join(",", this.productService?.GetDepartmentNames() ?? new string[]{})}");
    }

    public string ListenToCustomer() {

        var inquire = Console.ReadLine();
        while(string.IsNullOrEmpty(inquire)) {
            inquire = Console.ReadLine();
        }
        return inquire;
    }

    public void DisplaySearchResults(List<ProductSearchResult> searchResult) {

        Console.Write(CommonUtils.StringifyProductSearchResult(searchResult));        
        Console.WriteLine("Please review product search result and enter list of product SKUs and quantities, separated by colon. E.g. SKU1:2,SKU2:4 ");                   
        Console.WriteLine("If you are not satisfied with the search result, then feel free to search again.");
    }

    public VendingMachine SetupSearch(VendingMachine workflow,
                                                ConversationIntent intent) 
    {
        workflow.QueryEmbedding =
                    this.aiService?.GenerateEmbedding(intent.ProductDescription);
                
        workflow.Department = intent.Department;
        workflow.ProductService = productService;                
        return workflow;
    }

    
    public void StartConversation()
    {        
        //listen to customer
        var inquiry = ListenToCustomer();
        // try to understand what he wants    
        var aiResponse = this.aiService?.ExtractIntent(inquiry);

        
        workflow.Engage();

        while(aiResponse?.FreeText!=PromptHelper.TERMINATE) {

            if(aiResponse?.FreeText == PromptHelper.ABORT) {
                Console.WriteLine("Sorry, I can't help you here. Do you want to search again?");
                aiResponse = this.aiService?.ExtractIntent(ListenToCustomer());
                continue;
            }                

            if(CommonUtils.IsValid(aiResponse?.ConversationIntent)) {                                
                
                workflow = SetupSearch(workflow, aiResponse.ConversationIntent);
                workflow.SearchProduct();                   
                //if product search is empty, say, we do not have the product requested.                
                if(workflow.ProductSearchResults?.Count() == 0) {
                    Console.WriteLine("Sorry, we could not find any product matching your inquiry. Do you want to search again?");
                } else {                    
                    //display product search result                    
                    DisplaySearchResults(workflow.ProductSearchResults);
                    inquiry = ListenToCustomer();
                    var orderItems = CommonUtils.TryParseSKUs(inquiry, workflow.ProductSearchResults);
                    if(orderItems.Count() == 0) 
                    {
                        Log.Information("could not parse order items", inquiry);
                        aiResponse = this.aiService?.ExtractIntent(inquiry);
                        continue;
                    }
                    else {
                        workflow.AddProducts(orderItems);
                        //TODO: Thank you, please come again.
                        //Display transaction receipt.
                        break;
                    }      
                }                

            } else {

                Console.WriteLine("Sorry, I did not get that. Could you please repeat your query?");    
                aiResponse = this.aiService?.ExtractIntent(ListenToCustomer());
                continue;
            }

        }

        workflow.Finish();
    }
}
