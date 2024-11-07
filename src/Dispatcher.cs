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
    public AIResponse GetCustomerMessageAndPassIt2AI() 
    {
        string inquiry = ListenToCustomer();
        try {
            var aiResponse = this.aiService?.ExtractIntent(inquiry);            
            return aiResponse;
        } 
        catch(JsonException ex) {
            Log.Logger.Error(ex, inquiry);
            throw new ApplicationException(inquiry);
        } 
    }   
    public void DisplayMessages(List<MessageForCustomer> messages) 
    {   
        foreach (MessageForCustomer msg in messages) {
            Console.WriteLine(MessageForCustomer.Display(msg));    
        }
    }
    // public VendingMachine SetupSearch(VendingMachine workflow,
    //                     ConversationIntent intent, 
    //                     string inquiry) 
    // {
    //     workflow.QueryEmbedding =
    //                 this.aiService?.GenerateEmbedding(inquiry);

    //     Debug.Assert(workflow.QueryEmbedding!= null && workflow.QueryEmbedding.Length > 0);        
    //     workflow.Department = intent.Department;
    //     workflow.MinPrice = intent.minPrice;
    //     workflow.MaxPrice = intent.maxPrice;
    //     //workflow.ProductService = productService;                
    //     return workflow;
    // }
    public void StartConversation()
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

            // if(CommonUtils.IsValid(aiResponse?.ConversationIntent)) 
            // {        
            //     workflow = SetupSearch(workflow, aiResponse.ConversationIntent, inquiry);
            //     workflow.SearchProduct();                                                                   
            //     DisplayMessages(workflow.Messages);
                
            //     inquiry = ListenToCustomer();        
            //     aiResponse =  this.aiService?.ExtractIntent(inquiry);

            //     if(aiResponse?.FreeText == PromptHelper.ORDER_READY 
            //         && workflow.TryAddOrderItems(inquiry))                     
            //     {                        
            //         DisplayMessages(workflow.Messages);
            //         aiResponse = GetCustomerMessageAndPassIt2AI();        
            //         continue;
            //     } 
            //     else {
            //         Log.Information("No order items requested", inquiry);                        
            //         continue;
            //     }
                  
            // } else 
            // {
            //     Console.WriteLine("Sorry, I did not get that. Could you please repeat your query?");    
            //     aiResponse = this.aiService?.ExtractIntent(ListenToCustomer());
            //     continue;
            // }

        }

        workflow.Finish();
        DisplayMessages(workflow.Messages);        
    }
}
