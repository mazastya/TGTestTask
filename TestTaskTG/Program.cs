using TestTaskTG.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSingleton<MinesweeperService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyBuilder=>corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();