using OpenAI;
using Azure;
using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using System.Diagnostics;
using Serilog;
using OpenAI.Embeddings;

namespace StoreAgent;

public class OpenAIService : IAIService
{       
    private AzureOpenAIClient azureOpenAIClient;
    private ChatClient chatClient;
    private EmbeddingClient embeddingClient;
    private const string embeddingModel = "text-embedding-3-small";
    private const string completionModel = "gpt-4o-mini";

    public required string ChatEndpoint { get; set; }
    public required string EmbeddingEndpoint { get; set; }
    public required string Key { get; set; }

    public OpenAIService() {}


    public void Init() {

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information($"Endpoint: {this.ChatEndpoint}, Key: {this.Key}" );
        this.azureOpenAIClient = new(new Uri(this.ChatEndpoint),
                                    new ApiKeyCredential(this.Key));

        this.chatClient = this.azureOpenAIClient.GetChatClient(completionModel);                            
        this.embeddingClient = this.azureOpenAIClient.GetEmbeddingClient(embeddingModel);
    }
    public string ExtractIntent(string txt)
    {
        Debug.Assert(this.chatClient!=null);
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return result.Value.Content[0].Text;
    }

    public float[] GenerateEmbedding(string txt)
    {        
        Debug.Assert(this.embeddingClient!=null);        
        var result = embeddingClient.GenerateEmbedding(txt);        
        return result.Value.ToFloats().ToArray();
    }
    public bool GetUserConfirmation(string txt)
    {
        Debug.Assert(this.chatClient!=null);
        var result = this.chatClient.CompleteChat(new ChatMessage[]{txt});
        return bool.Parse(result.Value.Content[0].Text);
    }
}