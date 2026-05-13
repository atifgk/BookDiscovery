using BookDiscovery.Application;
using BookDiscovery.Application.Interfaces;
using BookDiscovery.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IBookSearchService, BookSearchService>();
builder.Services.AddScoped<IAiQueryParser, OpenAiQueryParser>();
builder.Services.AddScoped<IBookRankingService, BookRankingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://localhost:65037")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var appConfig = new AppConfiguration
{
    OpenAIAPIKey = builder.Configuration.GetSection("OpenAI:ApiKey").Get<string>() ?? string.Empty
};

builder.Services.AddSingleton(appConfig);

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
