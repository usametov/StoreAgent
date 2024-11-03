namespace StoreAgent.Models;

public enum ConversationTrigger {
    StartConversation,
    StartSearch,
    OrderReady,
    BackToSearch,
    TerminateConversation
}

public enum ConversationState {
        Off,
        On, 
        ProductLookup,
        AddToCart
}

