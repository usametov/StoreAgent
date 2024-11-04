##Application Design, Main Components


#Dispatcher 
(Boss) class that is facing customer. This class is an entry point for this application.

# Dispatcher Class Documentation

The `Dispatcher` class is responsible for managing the interaction between the user and the vending machine system. It initializes services, loads products, listens to customer inquiries, and handles the conversation workflow.

## Methods

### Init()

Initializes the `Dispatcher` by setting up the necessary services and configurations.

- **Services Initialized:**
  - `IAIService`: Handles AI-related tasks such as generating embeddings and extracting intents.
  - `IProductService`: Manages product-related operations such as adding and retrieving products.

- **Configuration:**
  - Retrieves Azure OpenAI API key, endpoint, and embedding endpoint from the configuration manager.
  - Initializes the AI service with the retrieved configurations.

### LoadProducts()

Loads products from a JSON file and adds them to the product service.

- **Steps:**
  - Deserializes products from a JSON file.
  - Generates embeddings for each product using the AI service.
  - Adds the products to the product service.

### ListenToCustomer()

Listens to customer input from the console.

- **Behavior:**
  - Continuously reads input from the console until a non-empty string is received.

### GetCustomerMessageAndPassIt2AI()

Gets customer input and passes it to the AI service to extract intent.

- **Steps:**
  - Calls `ListenToCustomer()` to get the customer's inquiry.
  - Passes the inquiry to the AI service to extract the intent.
  - Handles JSON exceptions and logs errors if they occur.

### DisplaySearchResults(List<ProductSearchResult> searchResult)

Displays the search results to the customer.

- **Steps:**
  - Converts the search results to a string format.
  - Writes the formatted results to the console.
  - Prompts the customer to enter a list of product SKUs and quantities.

### DisplayReceipt(List<OrderItem> items)

Displays the receipt to the customer after an order is ready.

- **Steps:**
  - Writes the total amount charged to the console.

### SetupSearch(VendingMachine workflow, ConversationIntent intent)

Sets up the search parameters for the vending machine.

- **Steps:**
  - Generates an embedding for the product description.
  - Sets the department, minimum price, and maximum price for the search.
  - Assigns the product service to the workflow.

### StartConversation()

Starts the conversation with the customer.

- **Steps:**
  - Engages the vending machine workflow.
  - Displays the greeting message to the customer.
  - Listens to customer inquiries and processes them.
  - Handles different conversation intents and triggers appropriate actions.
  - Terminates the conversation when the customer is done.

## Usage

To use the `Dispatcher` class, create an instance of it and call the `Init()` method to initialize the services. Then, call the `LoadProducts()` method to load the products and `StartConversation()` to begin interacting with the customer.
