using System.Reflection;
using FileUploadApi.Commands;
using FileUploadApi.Data;
using FileUploadApi.Models;
using FileUploadApi.Queries;
using FileUploadApi.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnection")));

builder.Services.AddScoped<IBlobRepo, BlobRepo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapGet("/api/files", async (IMediator mediator) =>{
    var query = new ListAllFilesQuery();
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

app.MapGet("/api/files/{fileName}", async (IMediator mediator, string fileName) =>{
    try
    {
        var query = new GetFileByNameQuery(fileName);

        var result = await mediator.Send(query);

        if(result.Content == null)
            return Results.NotFound();

        return Results.File(result.Content, result.ContentType, result.Name);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error downloading file: {ex.Message}");

        return Results.StatusCode(500);
    }
});

app.MapPost("/api/files", async (IMediator mediator, IFormFile file) =>{  
    var command = new UploadFileRequest(file);

    var result = await mediator.Send(command);
        
    return Results.Ok(result);
}).DisableAntiforgery();

app.MapDelete("/api/files/{fileName}", async (IMediator mediator, string fileName) =>{
    var command = new DeleteFileRequest(fileName);

    var result = await mediator.Send(command);

    return Results.Ok(result);
});

app.Run();

