using SearchEngine.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adding and configuring the services
builder.Services.ConfigureControllers();
builder.Services.ConfigureServices();
builder.Services.ConfigureCORS();
builder.Services.ConfigureSwagger();

//Building the configured app
var app = builder.Build();

// Configuring the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Using the required middlewares
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

//Running the app
app.Run();

