using OpenAI.Chat;
using System.Collections.Concurrent;

namespace WebApplication2
{
    public sealed class History
    {
        private readonly ConcurrentDictionary<string, IList<ChatMessage>> _store = new();

        public IReadOnlyList<ChatMessage> GetOrAddUserHistory(string userName, string message)
        {
            var chatMessages = _store.GetOrAdd(userName, _ => InitiateChatMessages());
            chatMessages.Add(new UserChatMessage(GenerateUserChatMessage(userName, message)));
            return chatMessages.AsReadOnly();
        }

        public void UpdateUserHistoryForAssistant(string userName, string message)
        {
            var chatMessages = _store.GetOrAdd(userName, _ => InitiateChatMessages());
            chatMessages.Add(new AssistantChatMessage(message));
        }

        private IList<ChatMessage> InitiateChatMessages()
        {
            return new List<ChatMessage>
            {
                new SystemChatMessage("""
                    You are a friendly and knowledgeable assistant participating in a group discussion. 
                    Your role is to provide helpful, accurate, and concise information when addressed. 
                    Maintain a respectful tone, ensure your responses are clear and relevant to the ongoing messages, and assist in facilitating productive discussions. 
                    Messages from users will be in the format 'UserName: chat message'. 
                    Pay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.
                """)
            };
        }

        private string GenerateUserChatMessage(string userName, string message)
        {
            return $"{userName}: {message}";
        }
    }
}
