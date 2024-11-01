namespace StoreAgent;
public interface IAIService {
    float[] GenerateEmbedding(string txt);
    string ExtractIntent(string txt);
    bool GetUserConfirmation(string txt);

    public string ChatEndpoint { get; set; }
    public string EmbeddingEndpoint { get; set; }
    public string Key { get; set; }

    public void Init(); 
}
