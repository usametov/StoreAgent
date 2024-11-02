namespace StoreAgent.Models;

public record ProductSearchResult {
    public required Product Product {get;set;}
    public required double Score {get;set;}
}