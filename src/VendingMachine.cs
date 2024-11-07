using OpenAI.VectorStores;
using Stateless;
using Stateless.Reflection;
using System.Diagnostics;
using StoreAgent.Models;
using StoreAgent.Helpers;
using StoreAgent.Services;
using Serilog;

namespace StoreAgent;

public class VendingMachine {

    public const int topK = 5;
    public const double threshold = 0.40;

    public float[]? QueryEmbedding {get;set;}

    public string? Department {get;set;}

    public IProductService? ProductService {get;set;}
    public IAIService? AIService {get;set;}

    public List<ProductSearchResult>? ProductSearchResults {get;set;}

    public List<OrderItem> OrderItems {get;set;}

    public decimal? OrderTotal {get;set;}

    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    public List<MessageForCustomer> Messages {get;set;}      

    private readonly StateMachine<ConversationState, ConversationTrigger> 
            workflow = new(ConversationState.Off);

    public VendingMachine() {
        workflow.Configure(ConversationState.Off)
                .Permit(ConversationTrigger.StartConversation, ConversationState.On)
                .Permit(ConversationTrigger.StartSearch, ConversationState.ProductLookup);

        workflow.Configure(ConversationState.On)                
                .Permit(ConversationTrigger.StartSearch, ConversationState.ProductLookup)
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off);

        workflow.Configure(ConversationState.ProductLookup)
                .Permit(ConversationTrigger.OrderReady, ConversationState.AddToCart)               
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off)
                .Permit(ConversationTrigger.StartConversation, ConversationState.On)
                .PermitReentry(ConversationTrigger.StartSearch);

        workflow.Configure(ConversationState.AddToCart)                 
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off)
                .Permit(ConversationTrigger.BackToSearch, ConversationState.ProductLookup)
                .Permit(ConversationTrigger.StartSearch, ConversationState.ProductLookup);
        
        OrderItems = [];    
        Messages = [];    
    }

    public void Engage() 
    {
        var promptHelper = new PromptHelper(ProductService?.GetDepartmentNames() ?? new string[]{}); 
        Debug.Assert(promptHelper.Departments.Count() > 0);

        Messages.Clear();
        Messages.Add(new MessageForCustomer.SimpleMessage(promptHelper.Greeting()));        
        workflow.Fire(ConversationTrigger.StartConversation);        
    }

    public void SearchProduct() {

        Debug.Assert(QueryEmbedding!=null && QueryEmbedding.Length > 0 
                     && !string.IsNullOrEmpty(Department));                

        Messages.Clear();
        workflow.Fire(ConversationTrigger.StartSearch);
        ProductSearchResults = ProductService?.GetSimilarProducts(this.QueryEmbedding, 
                this.Department, topK, MinPrice, MaxPrice, threshold);

        if(ProductSearchResults?.Count == 0) {
            Messages.Add(new MessageForCustomer.SimpleMessage(
                "Sorry, we could not find any product matching your inquiry. "
               +"Please let me know if you want to search again or terminate up."));

            workflow.Fire(ConversationTrigger.StartConversation);                                            
        }else
        {
            Debug.Assert(ProductSearchResults!=null);
            Messages.Add(new MessageForCustomer.SearchResult(ProductSearchResults));
            Messages.Add(new MessageForCustomer.SimpleMessage(System.Environment.NewLine));
            Messages.Add(new MessageForCustomer.SimpleMessage(
                            "Please review product search result and enter list of product SKUs and quantities, "
                            + "separated by colon. E.g. SKU1:2,SKU2:4 "));                   
            Messages.Add(new MessageForCustomer.SimpleMessage("If you are not satisfied with the search result, then feel free to search again."));
        }             
    }

    public void Finish() {
        workflow.Fire(ConversationTrigger.TerminateConversation);
        Messages.Clear();
        Messages.Add(
            new MessageForCustomer.SimpleMessage("Thank you, and please come again."));                   
    }
    public void AddOrderItems(List<OrderItem> items) 
    {        
        OrderItems.AddRange(items);
        workflow.Fire(ConversationTrigger.OrderReady);
        OrderTotal = OrderItems.Select(o=>o.Product.Price*o.Quantity).Sum();
        Messages.Clear();                       
        Messages.Add(new MessageForCustomer.SimpleMessage("Your order is ready."));
        Messages.Add(new MessageForCustomer.SimpleMessage("Order Total: "));
        Messages.Add(new MessageForCustomer.SimpleMessage(string.Format("{0:C}", OrderTotal) + System.Environment.NewLine));
    }

    public void AddOrder(string inquiry) 
    {
        Debug.Assert(ProductSearchResults?.Count() > 0);
        var orderItems = CommonUtils.TryParseSKUs(inquiry, ProductSearchResults);
        if(orderItems.Count() == 0) 
        {         
            Messages.Add(new MessageForCustomer.SimpleMessage("No order items requested"));                        
            //Log.Information("No order here: " + inquiry);
        } else 
        {
            AddOrderItems(orderItems);
            Messages.Add(new MessageForCustomer.SimpleMessage("Do you want to search more products?"));
        }
    }

    public StateMachineInfo GetInfo() {
        return workflow.GetInfo();        
    }

    public void ProcessIntent(AIResponse aiResponse, string inquiry) {

        Messages.Clear();

        if(aiResponse?.FreeText == PromptHelper.ABORT) {
            Messages.Add(new MessageForCustomer.SimpleMessage(
                        "Sorry, I can't help you here. Do you want to search again?"));
            return;
        } 
        else if(CommonUtils.IsValid(aiResponse?.ConversationIntent))
        {
            QueryEmbedding = AIService?.GenerateEmbedding(inquiry);
            Department = aiResponse.ConversationIntent.Department;
            MinPrice = aiResponse.ConversationIntent.minPrice;
            MaxPrice = aiResponse.ConversationIntent.maxPrice;

            SearchProduct();
        } else if(aiResponse?.FreeText == PromptHelper.ORDER_READY)                     
        {   
            AddOrder(inquiry);
        } else
        {   
            Messages.Add(new MessageForCustomer.SimpleMessage(
                "Sorry, I did not get that. Could you please repeat your query?"));    
        }
    }
}
