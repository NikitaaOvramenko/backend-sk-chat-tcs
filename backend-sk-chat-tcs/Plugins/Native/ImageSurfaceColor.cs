using backend_sk_chat_tcs.Models;
using dotenv.net;
using Microsoft.SemanticKernel;
using Supabase;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace backend_sk_chat_tcs.Plugins.Native
{
    public class ImageSurfaceColor
    {
        private readonly Supabase.Client supabase;

        public ImageSurfaceColor(Supabase.Client supabase)
        {
            this.supabase = supabase;
        }

        [KernelFunction, Description(
            "Edits an image based on a single user instruction. " +
            "If the user asks for multiple edits in one request, combine them into one operation. " +
            "Do NOT invoke this function more than once per request."
        )]
        public async Task<ResponseFormat> EditImageAsync(
            [Description("Instruction for which object to color to what color")] string instruction,
            [Description("It's Public Url for a recent image uploaded")] string publicUrl)
        {
            DotEnv.Load();
            var fileName = Environment.GetEnvironmentVariable("PYTHON_CHOICE");
            try
            {
                
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "Native", "script.py");

                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = $"\"{scriptPath}\" \"{publicUrl}\" \"{instruction}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                };

                using var process = Process.Start(psi);
                if (process == null)
                {
                    return new ResponseFormat { Message = "❌ Failed to start Python process.", Url = null };
                }

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    return new ResponseFormat { Message = $"⚠️ Python error: {error}", Url = null };
                }

                // Decode Base64 string from Python
                var imageBytes = Convert.FromBase64String(output.Trim());

                // Build unique name
                var uniqueName = $"Temp/{Guid.NewGuid()}.png";

                // Upload to Supabase
                await supabase.Storage
                    .From("media")
                    .Upload(imageBytes, uniqueName, new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        Upsert = false
                    });

                // Get public URL
                var publicUrl1 = supabase.Storage.From("media").GetPublicUrl(uniqueName);

                return new ResponseFormat
                {
                    Message = $"✅ Image updated with instruction: {instruction}",
                    Url = publicUrl1
                };
            }
            catch (Exception ex)
            {
                return new ResponseFormat
                {
                    Message = $"❌ Exception while processing image: {ex.Message}",
                    Url = null
                };
            }
        }
    }
}
