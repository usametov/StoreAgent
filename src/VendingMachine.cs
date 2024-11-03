using System.Runtime.CompilerServices;
using OpenAI.VectorStores;
using Stateless;
using System.Diagnostics;
using StoreAgent.Models;
using StoreAgent.Services;

namespace StoreAgent;
public class VendingMachine {

    public const int topK = 5;

    public float[]? QueryEmbedding {get;set;}

    public string? Department {get;set;}

    public IProductService? ProductService {get;set;}

    public List<ProductSearchResult>? ProductSearchResults {get;set;}

    public List<OrderItem> OrderItems {get;set;}

    public decimal? OrderTotal {get;set;}      

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
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off);

        workflow.Configure(ConversationState.AddToCart)                 
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off)
                .Permit(ConversationTrigger.BackToSearch, ConversationState.ProductLookup);
        
        OrderItems = new List<OrderItem>();        
    }

    public void Engage() {
        var promptHelper = new PromptHelper(ProductService?.GetDepartmentNames() ?? new string[]{}); 
        Debug.Assert(promptHelper.Departments.Count() > 0);
        Console.WriteLine(promptHelper.Greeting());
        workflow.Fire(ConversationTrigger.StartConversation);        
    }

    public void SearchProduct() {

        Debug.Assert(QueryEmbedding!=null && QueryEmbedding.Length > 0 
                     && !string.IsNullOrEmpty(Department));                

        workflow.Fire(ConversationTrigger.StartSearch);
        ProductSearchResults = ProductService?.GetSimilarProducts(this.QueryEmbedding, this.Department, topK);                      
    }

    public void Finish() {
        workflow.Fire(ConversationTrigger.TerminateConversation);
    }

    public void AddOrderItems(List<OrderItem> items) {        
        OrderItems.AddRange(items);
        workflow.Fire(ConversationTrigger.OrderReady);
        OrderTotal = OrderItems.Select(o=>o.Product.Price*o.Quantity).Sum();
    }
}
