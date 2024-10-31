namespace StoreAgent;
public interface IAIService {
    ReadOnlyMemory<float> GenerateEmbedding(string txt);
    string ExtractIntent(string txt);
    bool GetUserConfirmation(string txt);

    public string Endpoint { get; set; }
    public string Key { get; set; }

    public void Init(); 
}
