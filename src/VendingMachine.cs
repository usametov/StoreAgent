using System.Runtime.CompilerServices;
using OpenAI.VectorStores;
using Stateless;
using System.Diagnostics;
using StoreAgent.Models;
using StoreAgent.Helpers;
using StoreAgent.Services;

namespace StoreAgent;
public class VendingMachine {

    public const int topK = 5;
    public const double threshold = 0.40;

    public float[]? QueryEmbedding {get;set;}

    public string? Department {get;set;}

    public IProductService? ProductService {get;set;}

    public List<ProductSearchResult>? ProductSearchResults {get;set;}

    public List<OrderItem> OrderItems {get;set;}

    public decimal? OrderTotal {get;set;}

    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    public string? MessageForCustomer {get;set;}      

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
        
        OrderItems = new List<OrderItem>();        
    }

    public void Engage() {
        var promptHelper = new PromptHelper(ProductService?.GetDepartmentNames() ?? new string[]{}); 
        Debug.Assert(promptHelper.Departments.Count() > 0);
        MessageForCustomer = promptHelper.Greeting();
        workflow.Fire(ConversationTrigger.StartConversation);        
    }

    public void SearchProduct() {

        Debug.Assert(QueryEmbedding!=null && QueryEmbedding.Length > 0 
                     && !string.IsNullOrEmpty(Department));                

        workflow.Fire(ConversationTrigger.StartSearch);
        ProductSearchResults = ProductService?.GetSimilarProducts(this.QueryEmbedding, 
                this.Department, topK, MinPrice, MaxPrice, threshold);

        if(ProductSearchResults?.Count() == 0) {
            MessageForCustomer = "Sorry, we could not find any product matching your inquiry. "
                                +"Please let me know if you want to search again or terminate up.";

            workflow.Fire(ConversationTrigger.StartConversation);                                            
        }            
    }

    public void Finish() {
        workflow.Fire(ConversationTrigger.TerminateConversation);
    }

    public void AddOrderItems(List<OrderItem> items) {        
        OrderItems.AddRange(items);
        workflow.Fire(ConversationTrigger.OrderReady);
        OrderTotal = OrderItems.Select(o=>o.Product.Price*o.Quantity).Sum();
    }

    public bool TryAddOrderItems(string inquiry) 
    {
        Debug.Assert(ProductSearchResults?.Count() > 0);
        var orderItems = CommonUtils.TryParseSKUs(inquiry, ProductSearchResults);
        if(orderItems.Count() == 0) 
            return false;
        else 
        {
            AddOrderItems(orderItems);
            this.MessageForCustomer = "Do you want to search more products?";
            return true;
        }
    }
}
