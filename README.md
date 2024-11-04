# StoreAgent
This is a little demo of Agent app operating Vending Machine.

This Agent app interprets customer requests and answers questions for a general store similar to Walmart. A customer may ask a question that the LLM interprets and routes to a specific department to show availability and prices for asked by the customer using the LLM.

This app showcases the Agent controlling a Vending Machine in a simplified setup. Notably, it doesn't rely on any large-scale agent frameworks. The core design concept is centered around a 'Bring Your Own State Machine' approach.
This is due to the fact that we often prefer to avoid relying on probabilistic planning methods to manage our workflow. 

We also want to decouple probabilistic components from deterministic ones, keeping them isolated and distinct.

Before running this app, we should set evironment variables for AZURE_OPENAI_API_KEY and AZURE_OPENAI_ENDPOINT keys.

The current Product Service implementation is intentionally simplistic and should be replaced with a more robust product RAG component. Fortunately, this can be done seamlessly, thanks to our adherence to the Dependency Injection pattern.

If you plan to replace Azure Open AI, it's recommended to initially run the existing implementation for a period of time to gather sufficient log data. This data can then be used to fine-tune a smaller model, ensuring a smoother transition.

[Design writeup available here](https://github.com/usametov/StoreAgent/blob/main/docs/code-walkthrough.md)

