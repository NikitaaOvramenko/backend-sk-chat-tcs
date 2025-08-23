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

        public SessionController(ChatManager chatManager, SemanticKernel semanticKernel)
        {
            this.chatManager = chatManager;
            this.semanticKernel = semanticKernel;
        }

        [HttpPost("GetSession")]
        public string GetSessionIdAPI([FromBody] Session req)
        {
            var chat = chatManager.CreateChat(req.Id);

            chat.AddSystemMessage($"You are agent for TCS-PAINTS company, you will respond for this {req.Id}," +
                $"You only answer for questions related to Paints and how user want to paint them especially (eg. color this wall in green and door in black) etc and this company and also users relevant message like (user introduction, user QA, human stuff). " +
                $"If user asks something not related or off you respond like this: " +
                $"I appreciate your question but please ask question related to today's theme and topic which is paints. " +
                $"Also for example if user want you to modify(color) some surface from the image, YOU CAN DO THAT.");

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
                var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                filePath = Path.Combine(tempFolder, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Add info for AI only if image exists
                var chat = chatManager.GetChat(req.Id);
                chat.AddSystemMessage(
                    $"The uploaded image is stored as '{req.Image.FileName}'. " +
                    $"If the user asks to recolor or modify the image, call the function " +
                    $"ImageSurfaceColor.EditImageAsync with instruction = '{req.MessageT}' and imageName = '{req.Image.FileName}'."
                );
            }

            // Add user message regardless of image
            var chatForMessage = chatManager.GetChat(req.Id);
            chatManager.AddUserMessageToChat(chatForMessage, message);

            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var response = "";

            System.Diagnostics.Debug.Write($"Chat with ID: {req.Id}");

            var completion = semanticKernel.ChatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory: chatForMessage,
                executionSettings: settings,
                kernel: semanticKernel.GetKernel()
            );

            await foreach (var content in completion)
            {
                System.Diagnostics.Debug.Write(content.Content);
                response += content.Content;
            }

            chatManager.AddAIMessageToChat(chatForMessage, response);

            return response;
        }
    }
}
