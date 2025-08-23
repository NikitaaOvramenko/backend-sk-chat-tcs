using backend_sk_chat_tcs;
using dotenv.net;
using  Microsoft.SemanticKernel.Connectors.OpenAI;
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new SemanticKernel(
    Environment.GetEnvironmentVariable("MODEL_NAME"),
    Environment.GetEnvironmentVariable("OPENAI_APIKEY")
));
builder.Services.AddSingleton<ChatManager>();







var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
