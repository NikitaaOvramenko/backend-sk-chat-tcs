using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace backend_sk_chat_tcs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ChatManager chatManager;
        private SemanticKernel semanticKernel;
        public SessionController(ChatManager chatManager,SemanticKernel semanticKernel)
        {
            this.chatManager= chatManager;
            this.semanticKernel = semanticKernel;
        }

        [HttpPost("GetSession")]
        
        public string GetSessionIdAPI([FromBody] Session req)
        {

           var chat = chatManager.CreateChat(req.Id);


            chat.AddSystemMessage($"You are agent for TCS-PAINTS company, you will respond for this {req.Id}," +
                $"You only answer for questions related to Paints and this company and also users relevant message like ( user introduction, user QA, human stuff) if user asks something not related or off you respond like this:" +
                $"I appreciate your question but please ask question related to today's theme and topic which is paints.  " +
                $"this is just an example but you got an idea " +
                $"also for example if user want you to modify(color) some surface from the image, YOU CAN DO THAT ");



            chatManager.AddChat(chat);
           
              
            return req.Id;
        }

        [HttpPost("EndChat")]

        public string EndSessionAPI([FromBody] Session req)
        {
            var chat  = chatManager.GetChat(req.Id);
            chatManager.RemoveChat(chat);
            return $"{req.Id} ID Session is Closed !";
        }


        [HttpPost("WriteToChat")]
        public async Task<string> WriteToChat([FromBody] Message req)
        {
            var id = req.Id;
            var messageText = req.MessageText;
            var message = new ChatMessageContentItemCollection {

                new TextContent(messageText),
                new ImageContent(new Uri("https://images.unsplash.com/photo-1495578942200-c5f5d2137def?q=80&w=1475&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"))
            };
            var chat = chatManager.GetChat(id);
            chatManager.AddUserMessageToChat(chat, message);

            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var response = "";

            System.Diagnostics.Debug.Write($"Chat with ID: {id}");


            var completion = semanticKernel.ChatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory: chat, executionSettings: settings, kernel: semanticKernel.GetKernel());


            await foreach (var content in completion)
            {
                System.Diagnostics.Debug.Write(content.Content);
                response += content.Content;
               
                
            }

            chatManager.AddAIMessageToChat(chat, response);

            return response;
        }



    }
}
