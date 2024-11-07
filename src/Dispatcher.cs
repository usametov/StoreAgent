using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;
using StoreAgent.Helpers;
using StoreAgent.Models;
using StoreAgent.Services;
using System.Text.Json;
using System;
using Microsoft.VisualBasic;

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

        workflow.ProductService = this.productService;
        workflow.AIService = this.aiService;

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information("done Init");
    }

    public void LoadProducts() 
    {
        Debug.Assert(this.aiService!=null);
        Debug.Assert(this.productService!=null);

        var productDB = "/workspaces/StoreAgent/src/Repositories/Products.json";
        var products = CommonUtils.DeserializeProductsFromJsonFile(productDB);    
        Log.Information($"loaded {products.Count} products");
       
        var inflatedProducts = CommonUtils.InflateProductEmbeddings(products, this.aiService);        
        this.productService.AddAllProducts(inflatedProducts);
        
        var promptHelper = new PromptHelper(productService.GetDepartmentNames() ?? new string[]{}); 
        this.aiService.SystemPrompt = promptHelper.GetSystemPrompt();        
    }

    public string ListenToCustomer() {

        var inquiry = Console.ReadLine();
        while(string.IsNullOrEmpty(inquiry)) {
            inquiry = Console.ReadLine();
        }
        return inquiry;
    }
    public void DisplayMessages(List<MessageForCustomer> messages) 
    {   
        foreach (MessageForCustomer msg in messages) {
            Console.WriteLine(MessageForCustomer.Display(msg));    
        }
    }
    
    public void StartVendingMachine()
    {   
        workflow.Engage();
        DisplayMessages(workflow.Messages);        
        //get request from customer to pass it to AI        
        string inquiry = ListenToCustomer();
        var aiResponse = this.aiService?.ExtractIntent(inquiry);       

        while(aiResponse?.FreeText!=PromptHelper.TERMINATE) {

            workflow.ProcessIntent(aiResponse, inquiry);
            DisplayMessages(workflow.Messages);

            inquiry = ListenToCustomer();
            aiResponse = this.aiService?.ExtractIntent(inquiry);                       
        }

        workflow.Finish();
        DisplayMessages(workflow.Messages);        
    }
}
