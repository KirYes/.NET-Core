using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace WebApplication2
{
    public static class OpenAIExtensions
    {
        public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<OpenAIOptions>(configuration.GetSection("OpenAI"))
                .AddSingleton<ChatClient>(provider =>
                {
                    var options = provider.GetRequiredService<IOptions<OpenAIOptions>>().Value;

                    ArgumentException.ThrowIfNullOrWhiteSpace(options.Endpoint);
                    ArgumentException.ThrowIfNullOrWhiteSpace(options.Key);

                    AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(options.Endpoint), new ApiKeyCredential(options.Key));
                    return azureClient.GetChatClient(options.DeploymentName);
                });

        }
    }
}

