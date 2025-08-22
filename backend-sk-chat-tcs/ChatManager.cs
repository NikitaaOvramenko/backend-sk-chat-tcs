namespace backend_sk_chat_tcs
{

    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    using static System.Runtime.InteropServices.JavaScript.JSType;

   
    public class ChatManager
    {
        private Dictionary<string,ChatHistoryWrapper> chats1;


        public ChatManager() {

            chats1 = new Dictionary<string,ChatHistoryWrapper>();
        }

        public List<ChatHistoryWrapper> GetChats()
        {
            return chats1.Values.ToList();
        }

        public ChatHistoryWrapper CreateChat(string id)
        {
            ChatHistoryWrapper chatHistory = new ChatHistoryWrapper(id);

            return chatHistory;
        }

        public void AddChat(ChatHistoryWrapper chat)
        {

            //chats.Add(chat);
            chats1.Add(chat.GetID(),chat);
            System.Diagnostics.Debug.WriteLine($"Chat Added !");
            GetChats().ForEach(chat => System.Diagnostics.Debug.WriteLine(chat.GetID()));
        }


        public void RemoveChat(ChatHistoryWrapper chat)
        {

            
                chats1.Remove(chat.GetID());
                System.Diagnostics.Debug.WriteLine($"Chat Removed !");
                GetChats().ForEach(chat => System.Diagnostics.Debug.WriteLine(chat.GetID()));
            
        }

        public ChatHistoryWrapper GetChat(string id)
        {
            //foreach (var chat in chats)
            //{
            //    if(chat.GetID() == id)
            //    {
            //        return chat;
            //    }
            //}

            if (chats1.TryGetValue(id, out var chat1))
            {
                return chat1;
            }
            return null;


        }

        public void AddUserMessageToChat(ChatHistoryWrapper chat,string message)
        {
            chat.AddUserMessage(message);
        }

        public void AddUserMessageToChat(ChatHistoryWrapper chat, ChatMessageContentItemCollection message)
        {
            chat.AddUserMessage(message);
        }

        public void AddAIMessageToChat(ChatHistoryWrapper chat, string message)
        {
            chat.AddAssistantMessage(message);
        }

        


    }
}
