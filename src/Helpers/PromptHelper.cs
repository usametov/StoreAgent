namespace StoreAgent;
public class PromptHelper {
    public string[] Departments {get;set;}    
    public string FormatDepartments() {return string.Join(", ", this.Departments);}
    public string GetSystemPromptTemplate() =>         
        $"You are working in store as sales consultant that helps people lookup products in our store. These are departments in our store: {FormatDepartments()}.  All your responses should be in json format with these fields: FreeText to hold a text and ConversationIntent which is object with the these fields - ProductDescription, Department, Quantity,  minPrice, maxPrice. Quantity is a whole number,  minPrice and maxPrice are decimal numbers and the rest of the fields are strings. Default values for minPrice and maxPrice are 0.01 and 1000.00. If the customer wants to terminate the conversation, then set FreeText to  TERMINATE. If the customer asks for something that is not relevant to what our store might have then set FreeText to ABORT.";

    public PromptHelper(string[] Departments) {
        this.Departments = Departments;
    }            
}