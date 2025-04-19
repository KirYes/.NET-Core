using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;

namespace WebApplication2.Hubs
{
    public class ChatHub(ChatClient chatClient) : Hub
    {
        private ChatMessage? _chatMessage;
            
        public async Task SendMessage(string user, string message)
        {
            if (message.StartsWith("@gpt"))
            {
                await Clients.All.SendAsync("Question", user, message.Substring(4).Trim());
                _chatMessage  = new UserChatMessage(message);
                string result = chatClient.CompleteChat(_chatMessage).Value.Content[0].Text;
                await Clients.All.SendAsync("Answer", user, result);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}
