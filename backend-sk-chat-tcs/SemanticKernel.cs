namespace backend_sk_chat_tcs
{
    using backend_sk_chat_tcs.Plugins.Native;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using System.Reflection;

    public class SemanticKernel
    {
        private readonly string modelName;
        private readonly string apiKey;
        private readonly Kernel kernel;
        private readonly IChatCompletionService chatService;

        public SemanticKernel(string modelName, string apiKey)
        {
            this.modelName = modelName;
            this.apiKey = apiKey;

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(modelName, apiKey);


            builder.Plugins.AddFromType<Estimate>();

            //var pathToImageToImagePlugin = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "Semantic","ImageToImagePlugin");
            //builder.Plugins.AddFromPromptDirectory(pathToImageToImagePlugin,"ImageToImagePlugin");

            this.kernel = builder.Build(); 
            this.chatService = kernel.GetRequiredService<IChatCompletionService>();

            
        }
        public Kernel GetKernel() => this.kernel;
        public IChatCompletionService ChatCompletionService => this.chatService;
    }

}

