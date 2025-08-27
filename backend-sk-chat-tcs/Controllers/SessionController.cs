using backend_sk_chat_tcs.Models;
using dotenv.net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace backend_sk_chat_tcs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ChatManager chatManager;
        private SemanticKernel semanticKernel;
        private Supabase.Client supabase;

        public SessionController(ChatManager chatManager, SemanticKernel semanticKernel, Supabase.Client supabase)
        {
            this.chatManager = chatManager;
            this.semanticKernel = semanticKernel;
            this.supabase = supabase;
        }

        [HttpPost("GetSession")]
        public async Task<string> GetSessionIdAPI([FromBody] Session req)
        {
            var chat =  chatManager.CreateChat(req.Id);

            chat.AddSystemMessage($"You are agent for TCS-PAINTS company, you will respond for this {req.Id}," +
                $"You only answer for questions related to Paints and how user want to paint them especially (eg. color this wall in green and door in black) etc and this company and also users relevant message like (user introduction, user QA, human stuff). " +
                $"If user asks something not related or off you respond like this: " +
                $"I appreciate your question but please ask question related to today's theme and topic which is paints. " +
                $"Also for example if user want you to modify(color) some surface from the image, YOU CAN DO THAT." +
                $"Also Act Human and answer human like questions(eg. `hi``Hi my name is ...` `how are you ` and etc...)");

            chatManager.AddChat(chat);

            return req.Id;
        }

        [HttpPost("EndChat")]
        public string EndSessionAPI([FromBody] Session req)
        {
            var chat = chatManager.GetChat(req.Id);
            chatManager.RemoveChat(chat);
            return $"{req.Id} ID Session is Closed !";
        }

        [HttpPost("WriteToChat")]
        public async Task<string> WriteToChat([FromForm] Message req)
        {
            var id = req.Id;
            var messageText = req.MessageT;
            var file = req.Image;

            var message = new ChatMessageContentItemCollection
            {
                new TextContent(messageText),
                // We only add ImageContent explicitly if we want multimodal support later
            };

            string? filePath = null;

            // ✅ Handle image only if present
            if (file != null && file.Length > 0)
            {
               
                using var stream1 = file.OpenReadStream();
                using var ms = new MemoryStream();
                await stream1.CopyToAsync(ms);

                var bytes = ms.ToArray();
                var uniqueName = $"Temp/{Guid.NewGuid()}_{file.FileName}";

                await supabase.Storage
                    .From("media")
                    .Upload(bytes, uniqueName, new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        Upsert = false
                    });

                var publicUrl = supabase.Storage.From("media").GetPublicUrl(uniqueName);


                // Add info for AI only if image exists
                var chat = chatManager.GetChat(req.Id);
                chat.AddSystemMessage(
    $"The uploaded image is stored as '{publicUrl}'. " +
    $"If the user asks to recolor or modify the image, call the function " +
    $"ImageSurfaceColor.EditImageAsync with instruction = '{req.MessageT}' " +
    $"and imageName = '{uniqueName}'." +
    $"and Message should be `Image Updated!`" +
    $"IF USER UPLOADED JUST AN IMAGE WITHOUT PROMPT DESCRIBING WHAT TO DO WITH IT DONT CALL ImageSurfaceColor.EditImageAsync, OUTPUT_RESPONSE: Specify what color you want it to be painted ? " 
);

            }

            // Add user message regardless of image
            var chatForMessage = chatManager.GetChat(req.Id);
            chatManager.AddUserMessageToChat(chatForMessage, message);

            var settings = new OpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(ResponseFormat),
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions

            };

            var chatResponse = "";

            System.Diagnostics.Debug.Write($"Chat with ID: {req.Id}");

            var completion = semanticKernel.ChatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory: chatForMessage,
                executionSettings: settings,
                kernel: semanticKernel.GetKernel()
            );



            await foreach (var content in completion)
            {

      
                System.Diagnostics.Debug.Write(content.Content);
                chatResponse += content.Content;
            }

            chatManager.AddAIMessageToChat(chatForMessage, chatResponse);
                



            return chatResponse;
        }
    }
}
