using StoreAgent.Models;

namespace StoreAgent;
public class PromptHelper {

    public const string TERMINATE = "TERMINATE";
    public const string ABORT = "ABORT";
    public const string ORDER_READY = "ORDER_READY";

    public string Greeting() => 
        $"Welcome to our Square10 dollar store. We have everything under 100 bucks! Here is our departments list: {this.FormatDepartments()}. Please let me know what are you looking for. Do not forget to specify price range and quantity.";
    public string[] Departments {get;set;}    
    public string FormatDepartments() {return string.Join(", ", this.Departments);}
    public string GetSystemPromptTemplate() =>         
        $"You are working in store as sales consultant that helps people lookup products in our store. These are departments in our store: {FormatDepartments()}. "
        + " All your responses should be in json format with these fields: FreeText to hold a text and ConversationIntent which is object with the these fields - ProductDescription, Department, Quantity,  minPrice, maxPrice. Quantity is a whole number,  minPrice and maxPrice are decimal numbers and the rest of the fields are strings. Default values for minPrice and maxPrice are 0.01 and 1000.00. If the customer wants to terminate the conversation, then set FreeText to  TERMINATE. If the customer asks for something that is not relevant to what our store might have then set FreeText to ABORT."
        + "If customer query looks like sequence of SKU:Quantity pairs separated by comma then set FreeText to ORDER_READY. Here is an example of sequence of SKU:Quantity pairs: SKU12:1,RTX:3,WRT:5 - here SKU is random alphanumeric sequence of 3 to 8 characters.";

    public PromptHelper(string[] Departments) {
        this.Departments = Departments;
    }

                
}