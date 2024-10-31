using OpenAI;
using Azure;
using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;

namespace StoreAgent;

public class OpenAIService : IAIService
{       
    private AzureOpenAIClient azureOpenAIClient;
    private ChatClient chatClient;
    private const string embeddingModel = "text-embedding-3-small";
    private const string completionModel = "gpt-4o-mini";

    public OpenAIService(string endpoint, string key) {

        this.azureOpenAIClient = new(new Uri(endpoint),
                                    new ApiKeyCredential(key));

        this.chatClient = this.azureOpenAIClient.GetChatClient(completionModel);                            
    }
    public string ExtractIntent(string txt)
    {
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return result.Value.Content[0].Text;
    }

    public ReadOnlyMemory<float> GenerateEmbedding(string txt)
    {        
        var embeddingClient = this.azureOpenAIClient.GetEmbeddingClient(embeddingModel);
        var result = embeddingClient.GenerateEmbedding(txt);        
        return result.Value.ToFloats();
    }
    public bool GetUserConfirmation(string txt)
    {
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return bool.Parse(result.Value.Content[0].Text);
    }
}