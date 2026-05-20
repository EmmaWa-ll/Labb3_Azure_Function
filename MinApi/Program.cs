using MinApi.Endpoints;
using MinApi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Service
builder.Services.AddSingleton<CosmoService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapCustomerEndpoints();


app.Run();


