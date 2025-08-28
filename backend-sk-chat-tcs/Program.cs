using backend_sk_chat_tcs;
using dotenv.net;
using  Microsoft.SemanticKernel.Connectors.OpenAI;
using Supabase;
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



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
builder.Services.AddSingleton(provider => new Supabase.Client(Environment.GetEnvironmentVariable("SUPBASE_URL"), Environment.GetEnvironmentVariable("SUPBASE_KEY"), new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
}));







var app = builder.Build();

app.UseCors("AllowReactDev");

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
