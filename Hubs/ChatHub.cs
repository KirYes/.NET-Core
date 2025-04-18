using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Text;

namespace WebApplication2.Hubs
{
    public class ChatHub : Hub
    {
        private readonly History _history;
        private readonly OpenAIClient _openAI;
        private readonly OpenAIOptions _options;

        public ChatHub(History history, OpenAIClient openAI, IOptions<OpenAIOptions> options)
        {
            _history = history;
            _openAI = openAI;
            _options = options.Value;
        }

        public async Task SendMessage(string user, string message)
        {
            if (message.StartsWith("@gpt"))
            {
                var id = Guid.NewGuid().ToString();
                var actualMessage = message.Substring(4).Trim();

                var messagesWithHistory = _history.GetOrAddUserHistory(user, actualMessage);
                await Clients.All.SendAsync("NewMessage", user, message);

                var chatClient = _openAI.GetChatClient(_options.Model);
                var totalCompletion = new StringBuilder();
                var lastSentTokenLength = 0;

                await foreach (var completion in chatClient.CompleteChatStreamingAsync(messagesWithHistory))
                {
                    foreach (var content in completion.ContentUpdate)
                    {
                        totalCompletion.Append(content);

                        if (totalCompletion.Length - lastSentTokenLength > 20)
                        {
                            await Clients.All.SendAsync("newMessageWithId", "AI Assistant", id, totalCompletion.ToString());
                            lastSentTokenLength = totalCompletion.Length;
                        }
                    }
                }

                _history.UpdateUserHistoryForAssistant(user, totalCompletion.ToString());
                await Clients.All.SendAsync("newMessageWithId", "AI Assistant", id, totalCompletion.ToString());
            }
            else
            {
                _ = _history.GetOrAddUserHistory(user, message);
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}
