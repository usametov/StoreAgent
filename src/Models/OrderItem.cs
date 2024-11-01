namespace StoreAgent.Models;

public record OrderItem {
    public required Product Product{get;set;}
    public required uint Quantity {get;set;}
}