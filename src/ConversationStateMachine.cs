using System.Runtime.CompilerServices;
using OpenAI.VectorStores;
using Stateless;
using System.Diagnostics;
using StoreAgent.Models;
using StoreAgent.Services;

namespace StoreAgent;
public class ConversationStateMachine {

    public const int topK = 5;

    public float[]? QueryEmbedding {get;set;}

    public string? Department {get;set;}

    public IProductService? ProductService {get;set;}

    public List<ProductSearchResult>? ProductSearchResults {get;set;}

    public OrderItem? OrderItem {get;set;}

    private readonly StateMachine<ConversationState, ConversationTrigger> 
            workflow = new(ConversationState.Off);

    public ConversationStateMachine() {
        workflow.Configure(ConversationState.Off)
                .Permit(ConversationTrigger.StartConversation, ConversationState.On)
                .Permit(ConversationTrigger.StartSearch, ConversationState.ProductLookup);

        workflow.Configure(ConversationState.On)                
                .Permit(ConversationTrigger.StartSearch, ConversationState.ProductLookup)
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off);

        workflow.Configure(ConversationState.ProductLookup)
                .Permit(ConversationTrigger.IntentReady, ConversationState.AddToCart)               
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off);

        workflow.Configure(ConversationState.AddToCart)                 
                .Permit(ConversationTrigger.TerminateConversation, ConversationState.Off)
                .Permit(ConversationTrigger.BackToSearch, ConversationState.ProductLookup);
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
    public void AddProducts(List<OrderItem> items) {        
        workflow.Fire(ConversationTrigger.IntentReady);
    }
}