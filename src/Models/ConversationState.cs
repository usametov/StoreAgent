namespace StoreAgent.Models;

public enum ConversationTrigger {
    StartConversation,
    StartSearch,
    IntentReady,
    BackToSearch,
    TerminateConversation
}

public enum ConversationState {
        Off,
        On, 
        ProductLookup,
        AddToCart
}

