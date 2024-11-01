using System.Runtime.CompilerServices;
using OpenAI.VectorStores;
using Stateless;
using StoreAgent.Models;

namespace StoreAgent;
public class ConversationStateMachine {
    public ConversationIntent? Intent {get;set;}
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
        workflow.Fire(ConversationTrigger.StartConversation);
    }
    public void SearchProduct() {
        workflow.Fire(ConversationTrigger.StartSearch);
    }
    public void Finish() {
        workflow.Fire(ConversationTrigger.TerminateConversation);
    }
    public void AddProduct(OrderItem item) {        
        workflow.Fire(ConversationTrigger.IntentReady);
    }
}