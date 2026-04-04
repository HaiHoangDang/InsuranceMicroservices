using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

// Eureka
builder.Services.AddDiscoveryClient(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();