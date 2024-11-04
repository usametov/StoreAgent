# Main Components

# Dispatcher Class 

The `Dispatcher` class is responsible for managing the interaction between the user and the vending machine state machine. This class is an entry point for our app. It initializes services, loads products, listens to customer inquiries, and handles the conversation workflow. Think of it as a 'Boss' class which is facing customer. 

## Methods

### StartConversation()

This is main entry Starts the conversation with the customer.

- **Steps:**
  - Engages the vending machine workflow.
  - Displays the greeting message to the customer.
  - Listens to customer inquiries and processes them.
  - Handles different conversation intents and triggers appropriate actions.
  - Terminates the conversation when the customer is done.


### Init()

Initializes the `Dispatcher` by setting up the necessary services and configurations by using Microsoft Dependency injection framework.

- **Services Initialized:**
  - `IAIService`: Handles AI-related tasks such as generating embeddings and extracting intents.
  - `IProductService`: Manages product-related operations such as adding and retrieving products.

- **Configuration:**
  - Retrieves Azure OpenAI API key, endpoint, and embedding endpoint from the configuration manager.
  - Initializes the AI service with the retrieved configurations.

### LoadProducts()

Loads products from a JSON file and adds them to the product service.

### ListenToCustomer()

Gets customer input from the console.

### GetCustomerMessageAndPassIt2AI()

Gets customer input and passes it to the AI service to extract intent.

### DisplaySearchResults(List<ProductSearchResult> searchResult)

Displays the search results to the customer.

### DisplayReceipt(List<OrderItem> items)

Displays the receipt to the customer after an order is ready.
  - Writes the total amount charged to the console.

### SetupSearch(VendingMachine workflow, ConversationIntent intent)

Prepares our vending machine for product search.

# VendingMachine Class 

The `VendingMachine` class manages the state and workflow of the vending machine system. It is a state machine, which handles product search, order management, and conversation state transitions using the Stateless library. Think of it as an Agent brain. 
![Vendor Machine graph](./vending-machine.svg)

And here is our System prompt that communicates our state machine design to LLM:

```
You are working in store as sales consultant that helps people lookup products in our store. 
These are departments in our store: electronics, groceries, pharmacy, kids.  
All your responses should be in json format with these fields: 
FreeText to hold a text 
and ConversationIntent which is object with the these fields 
- ProductDescription, Department, Quantity,  minPrice, maxPrice. 
Quantity is a whole number,  minPrice and maxPrice are decimal numbers 
and the rest of the fields are strings. 
Default values for minPrice and maxPrice are 0.01 and 1000.00. 
If the customer wants to terminate the conversation, then set FreeText to  TERMINATE. 
If the customer asks for something that is not relevant to what our store might have then set FreeText to ABORT.
If customer query looks like sequence of SKU:Quantity pairs separated by comma then set FreeText to ORDER_READY. 
Here is an example of sequence of SKU:Quantity pairs: SKU12:1,RTX:3,WRT:5 - here SKU is random alphanumeric sequence of 3 to 8 characters.
```

## Methods

### Engage()

Engages the vending machine workflow by initializing the prompt helper and starting the conversation.

### SearchProduct()

Initiates a product search based on the query embedding and department.

### Finish()

Terminates the conversation and sets a thank you message for the customer.
  - Sets the message for the customer to thank them for using the service.

### AddOrderItems(List<OrderItem> items)

Adds order items to the vending machine and calculates the total order amount.

### TryAddOrderItems(string inquiry)

Attempts to parse and add order items from the customer's inquiry.

### GetInfo()

This is utility method that helps to plot the graph of our state machine.

The rest of the application is pretty standard. We have model classes, data holders. We have Service Contracts, Service implementations, and utilities.


