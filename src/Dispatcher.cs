using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;

namespace StoreAgent;
public class Dispatcher {
    private IAIService aiService;

    public Dispatcher() {        
    }

    public void Init() {
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new string[]{});
        builder.Services.AddSingleton<IAIService, OpenAIService>();
        using IHost host = builder.Build();
        host.RunAsync();
        Debug.Print("host is running");

        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        this.aiService = provider.GetRequiredService<IAIService>();
        
        this.aiService.Endpoint = ConfigurationManager.GetAzureOpenAIEndpoint();
        this.aiService.Key = ConfigurationManager.GetAzureOpenAIApiKey();
        this.aiService.Init();

        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
    
        Log.Information("done Init");
    }

    public string TestOpenAI(string txt) {
        Debug.Assert(this.aiService!=null);
        Log.Information("Entered TestOpenAI");
        return this.aiService.ExtractIntent(txt);
    }

}