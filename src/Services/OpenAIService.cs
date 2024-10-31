using OpenAI;
using Azure;
using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using System.Diagnostics;
using Serilog;

namespace StoreAgent;

public class OpenAIService : IAIService
{       
    private AzureOpenAIClient azureOpenAIClient;
    private ChatClient chatClient;
    private const string embeddingModel = "text-embedding-3-small";
    private const string completionModel = "gpt-4o-mini";

    public required string Endpoint { get; set; }
    public required string Key { get; set; }

    public OpenAIService() {}


    public void Init() {

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information($"Endpoint: {this.Endpoint}, Key: {this.Key}" );
        this.azureOpenAIClient = new(new Uri(this.Endpoint),
                                    new ApiKeyCredential(this.Key));

        this.chatClient = this.azureOpenAIClient.GetChatClient(completionModel);                            
    }
    public string ExtractIntent(string txt)
    {
        Debug.Assert(this.chatClient!=null);
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return result.Value.Content[0].Text;
    }

    public ReadOnlyMemory<float> GenerateEmbedding(string txt)
    {        
        Debug.Assert(this.azureOpenAIClient!=null);
        var embeddingClient = this.azureOpenAIClient.GetEmbeddingClient(embeddingModel);
        var result = embeddingClient.GenerateEmbedding(txt);        
        return result.Value.ToFloats();
    }
    public bool GetUserConfirmation(string txt)
    {
        Debug.Assert(this.chatClient!=null);
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return bool.Parse(result.Value.Content[0].Text);
    }
}