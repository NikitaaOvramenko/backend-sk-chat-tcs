using dotenv.net;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace backend_sk_chat_tcs.Plugins.Native
{
    public class ImageSurfaceColor
    {
        [KernelFunction, Description(
"Edits an image based on a single user instruction. If the user asks for multiple edits in one request, combine them into one operation. Do NOT invoke this function more than once per request."
)]

        public async Task<string> EditImageAsync([Description("Instruction for which object to color to what color")]string instruction,[Description("It's variable name for a recent image uploaded")]string imageName)
        {

            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "Native", "script.py");
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Temp", imageName);
            var callbackUrl = "http://localhost:8080/api/Test/"; // your endpoint

            var psi = new ProcessStartInfo
            {
                FileName = "python",                  // "python3" if that's your command
                Arguments = $"{scriptPath} \"{imagePath}\" \"{instruction}\" \"{callbackUrl}\"",              // your script
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            using var proc = Process.Start(psi)!;

            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();

            //Console.WriteLine("OUTPUT:\n" + output);
            //Console.WriteLine("ERROR:\n" + error);

            return "";
        }
    }
}
