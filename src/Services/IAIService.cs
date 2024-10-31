namespace StoreAgent;
public interface IAIService {
    ReadOnlyMemory<float> GenerateEmbedding(string txt);
    string ExtractIntent(string txt);
    bool GetUserConfirmation(string txt);
}
