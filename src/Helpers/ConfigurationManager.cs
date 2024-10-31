public class ConfigurationManager{

    public static string GetAzureOpenAIApiKey() {        
        return Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";
    }

    public static string GetAzureOpenAIEndpoint() {
        return Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
    }
}