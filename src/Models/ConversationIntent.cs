namespace StoreAgent.Models;

public class ConversationIntent
{
    public required string ProductDescription { get; set; }
    public required string Department { get; set; }
    public decimal minPrice { get; set; }
    public decimal maxPrice { get; set; }
}
