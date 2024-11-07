using StoreAgent.Helpers;
using StoreAgent.Models;

/// <summary>
/// poor man's UnionType (a.k.a Discriminated Union)
/// </summary>
public abstract class MessageForCustomer {

    public class SimpleMessage(string message) : MessageForCustomer
    {
        public string Message {get {return message;}}    
    }

    public class SearchResult(List<ProductSearchResult> searchResult) : MessageForCustomer 
    {
        public List<ProductSearchResult> Data {get {return searchResult;}}                    
    }

    public static string Display(MessageForCustomer msg) => msg switch {

        SimpleMessage m => m.Message,
        SearchResult sr => CommonUtils.StringifyProductSearchResult(sr.Data),
        _ => throw new NotSupportedException("Message Invalid")
    }; 
}