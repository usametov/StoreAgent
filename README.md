# StoreAgent
This is a little demo of Agent app operating Vending Machine.

This app showcases the Agent controlling a Vending Machine in a simplified setup. Notably, it doesn't rely on any large-scale agent frameworks. The core design concept is centered around a 'Bring Your Own State Machine' approach.

Before running this app, we should set evironment variables for AZURE_OPENAI_API_KEY and AZURE_OPENAI_ENDPOINT keys.

This little agent app interprets customer requests and answers questions for a general store similar to Walmart.
A customer may ask a question that the LLM interprets and routes to a specific department to show availability and prices for asked by the customer using the LLM.

[App design writeup is available here](https://github.com/usametov/StoreAgent/blob/main/docs/code-walkthrough.md)

