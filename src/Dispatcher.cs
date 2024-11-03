using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;
using StoreAgent.Helpers;
using StoreAgent.Models;
using StoreAgent.Services;

namespace StoreAgent;

public class Dispatcher {

    private IAIService? aiService;
    private IProductService? productService;
    private VendingMachine workflow = new VendingMachine();  


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
        
        Log.Information($"Department names: {String.Join(",", this.productService?.GetDepartmentNames() ?? new string[]{})}");
    }

    public string ListenToCustomer() {

        var inquire = Console.ReadLine();
        while(string.IsNullOrEmpty(inquire)) {
            inquire = Console.ReadLine();
        }
        return inquire;
    }

    public AIResponse GetCustomerMessageAndPassIt2AI() 
    {
        string inquiry = ListenToCustomer();
        return this.aiService?.ExtractIntent(inquiry);
    }

    public void DisplaySearchResults(List<ProductSearchResult> searchResult) 
    {
        Debug.Assert(searchResult!=null);
        Console.Write(CommonUtils.StringifyProductSearchResult(searchResult));        
        Console.WriteLine(System.Environment.NewLine);
        Console.WriteLine("Please review product search result and enter list of product SKUs and quantities, separated by colon. E.g. SKU1:2,SKU2:4 ");                   
        Console.WriteLine("If you are not satisfied with the search result, then feel free to search again.");
    }

    public void DisplayReceipt(List<OrderItem> items) 
    {   
        Console.WriteLine("Your order is ready.");
        Console.Write("Total amount charged: ");                   
        Console.Write(string.Format("{0:C}", workflow.OrderTotal));
        Console.WriteLine(System.Environment.NewLine);
        Console.WriteLine("Thank you, and please come again.");                   
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
        //get request from customer to pass it to AI        
        var aiResponse = GetCustomerMessageAndPassIt2AI();
        workflow.Engage();
        Console.WriteLine(workflow.MessageForCustomer);

        while(aiResponse?.FreeText!=PromptHelper.TERMINATE) {

            if(aiResponse?.FreeText == PromptHelper.ABORT) {
                Console.WriteLine("Sorry, I can't help you here. Do you want to search again?");
                aiResponse = GetCustomerMessageAndPassIt2AI();
                continue;
            }                

            if(CommonUtils.IsValid(aiResponse?.ConversationIntent)) 
            {        
                workflow = SetupSearch(workflow, aiResponse.ConversationIntent);
                workflow.SearchProduct();                                   
                //product search is empty, sorry   
                if(workflow.ProductSearchResults?.Count() == 0) {
                    Console.WriteLine(workflow.MessageForCustomer);
                } else {                    
                    //display product search result                    
                    DisplaySearchResults(workflow.ProductSearchResults);                    
                    string inquiry = ListenToCustomer();        
                    aiResponse =  this.aiService?.ExtractIntent(inquiry);

                    if(aiResponse?.FreeText == PromptHelper.ORDER_READY 
                        && workflow.TryAddOrderItems(inquiry))                     
                    {                        
                        DisplayReceipt(workflow.OrderItems);
                        Console.WriteLine(workflow.MessageForCustomer);
                        break;
                    } 
                    else {
                        Log.Information("No order items requested", inquiry);                        
                        continue;
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
