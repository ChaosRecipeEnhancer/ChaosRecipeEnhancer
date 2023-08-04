using System.Reflection;
using Amazon.Lambda.Core;
using ChaosRecipeEnhancer.API;
using ChaosRecipeEnhancer.API.Extensions;
using Microsoft.OpenApi.Models;

// TODO: This should be set (by... something).
var apiVersion = Environment.GetEnvironmentVariable("API_VERSION") ?? "latest";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        apiVersion,
        new OpenApiInfo
        {
            Title = $"Chaos Recipe Enhancer API {apiVersion}",
            Version = apiVersion,
            Description = "API for Chaos Recipe Enhancer",
            Contact = new OpenApiContact
            {
                Name = "Chaos Recipe Enhancer",
                Email = "api-support@chaos-recipe.com"
            }
        }
    );

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddHealthChecks();

// Application configuration settings
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseSwagger(swagger =>
{
    swagger.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        $"{apiVersion}/swagger.json",
        $"Chaos Recipe Enhancer API {apiVersion}"
    );
    options.RoutePrefix = $"swagger";
});

app.MapHealthChecks("/health");

app.Use(
    async (context, next) =>
    {
        var apiGatewayRequest = context?.GetAPIGatewayProxyRequest();
        var transactionId =
            apiGatewayRequest?.RequestContext.RequestId ?? Guid.NewGuid().ToString();

        PowerToolsLogger.LogContext(
            apiGatewayRequest,
            context
                ?.Items
                ?[Amazon.Lambda.AspNetCoreServer.AbstractAspNetCoreFunction.LAMBDA_CONTEXT]
                as ILambdaContext,
            transactionId
        );

        context!.Items["transactionId"] = transactionId;
        context!.Response.Headers["X-TransactionId"] = transactionId;

        await next(context);
    }
);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet(
        "/",
        async context =>
        {
            context.Response.Redirect("/swagger");
        }
    );
});

// Entry point for API
app.Run();
