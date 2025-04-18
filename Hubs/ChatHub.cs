using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;
using Azure.Core;
using Azure.AI.OpenAI;
using Azure;
using System.ClientModel.Primitives;

namespace WebApplication2.Hubs
{
    public class ChatHub : Hub
    {
     
        public async Task SendMessage(string user, string message)
        {
            var endpoint = new Uri("https://kyryl-m9ndonei-swedencentral.cognitiveservices.azure.com/");
            var model = "gpt-4o";
            var deploymentName = "gpt-4o";
            var apiKey = "3PPWk2AfHud6U1r9wKzWapBqeu64mSenNyZ2qiHgqsbnLQq9O4sdJQQJ99BDACfhMk5XJ3w3AAAAACOGg6Y5";
            AzureOpenAIClient azureClient = new(
       endpoint,
       new AzureKeyCredential(apiKey));
            ChatClient chatClient = azureClient.GetChatClient(deploymentName);
            var requestOptions = new ChatCompletionOptions()
            {
                MaxOutputTokenCount = 4096,
                Temperature = 1.0f,
                TopP = 1.0f
            };
            if (message.StartsWith("@gpt"))
            {
                await Clients.All.SendAsync("Question", user, message.Substring(4).Trim());
                List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful assistant."),
                    new UserChatMessage(message),
};
                var response = chatClient.CompleteChat(messages, requestOptions);
                await Clients.All.SendAsync("Message", user, response.Value.Content[0].Text);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}
