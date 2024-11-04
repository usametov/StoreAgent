# StoreAgent
This is a little demo of Agent app operating Vending Machine.

This Agent app interprets customer requests and answers questions for a general store similar to Walmart. A customer may ask a question that the LLM interprets and routes to a specific department to show availability and prices for asked by the customer using the LLM.

This app showcases the Agent controlling a Vending Machine in a simplified setup. Notably, it doesn't rely on any large-scale agent frameworks. The core design concept is centered around a 'Bring Your Own State Machine' approach.
There are multiple reasons for this approach:
1. we want to avoid relying on probabilistic planning methods to manage our workflow. 
2. We want to avoid falling into invalid state.  
3. We want to decouple probabilistic components from deterministic ones, keeping them isolated and distinct.

Our state machine is implemented using Stateless library.The Stateless library offers an 'Export to DOT graph' feature, enabling the runtime visualization of state machines. This approach ensures that the code remains the single source of truth, while state diagrams are automatically generated as up-to-date by-products. This makes it easier to communicate the business logic between developers and business teams. 

Before running this app, we should set evironment variables for AZURE_OPENAI_API_KEY and AZURE_OPENAI_ENDPOINT keys. 
Generate keys on the Azure Portal. 
> The Keys & Endpoint section can be found in the Resource Management section. 
> Copy your endpoint and access key as you'll need both for authenticating your API calls. 
> You can use either KEY1 or KEY2. 
> Always having two keys allows you to securely rotate and regenerate keys without causing a service disruption.
![Create Deployment on Azure Portal](https://learn.microsoft.com/en-us/azure/ai-services/openai/media/quickstarts/endpoint.png)

The current Product Service implementation is intentionally simplistic and should be replaced with a more robust product RAG component. This can be done seamlessly, as we are using to the Dependency Injection pattern.

If you plan to replace Azure Open AI, it's recommended to initially run the existing implementation for a period of time to gather sufficient log data. This data can then be used to fine-tune a smaller model, ensuring a smoother transition. This implementation is utilizing the gpt-4o-mini model, the cheapest model, and our token usage is low. And for embeddings, we're using text-3-small here.

[Design writeup available here](https://github.com/usametov/StoreAgent/blob/main/docs/code-walkthrough.md)

