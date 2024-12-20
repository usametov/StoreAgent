using StoreAgent.Models;

namespace StoreAgent.Services;

public interface IAIService {
    
    float[] GenerateEmbedding(string txt);
    
    public AIResponse ExtractIntent(string txt);
    
    bool GetUserConfirmation(string txt);

    public string ChatEndpoint { get; set; }
    
    public string EmbeddingEndpoint { get; set; }

    public string SystemPrompt {get;set;}

    public string Key { get; set; }

    public void Init(); 
}
