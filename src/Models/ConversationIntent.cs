namespace StoreAgent.Models;

public class ConversationIntent
{
    public string ProductDescription { get; set; }
    public string Department { get; set; }
    public decimal minPrice { get; set; }
    public decimal maxPrice { get; set; }
}
