using ProductsApi.Exceptions.Handling;
using ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ProductService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Catches exceptions thrown by anything below it and hands them to
// GlobalExceptionHandler. For the Part 2 "raw 500" checkpoint, comment
// this line out temporarily.
app.UseExceptionHandler();

// Turns empty error responses (for example an unknown route's 404)
// into Problem Details too, so every error has the same shape.
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
